using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProcastinationKiller.Helpers;
using ProcastinationKiller.Models;
using ProcastinationKiller.Models.Responses;
using ProcastinationKiller.Models.Responses.Abstract;
using ProcastinationKiller.Services.Abstract;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ProcastinationKiller.Services
{
    /// <summary>
    /// Interfejs dla seriwsu obsługujcego logowanie, rejestracje, autentyfikacje
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Autnetyfikuj
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        User Authenticate(string username, string password, DateTime currentTime);

        /// <summary>
        /// Pobierz wszystkich użytkiowników
        /// </summary>
        /// <returns></returns>
        IEnumerable<object> GetAll();

        /// <summary>
        /// Zarejestruj nowego użytkownika
        /// </summary>
        /// <param name="registrationModel"></param>
        /// <returns></returns>
        Task<IValidationState> RegisterUser(UserRegistrationModel registrationModel);

        /// <summary>
        /// Aktywuj konto nowego użytkownika
        /// </summary>
        /// <param name="secret"></param>
        /// <returns></returns>
        Task<IValidationState> ActivateAccount(string secret);

        void AddTodo(string description, bool isCompleted, string name, int userId, DateTime regdate, DateTime targetDate);

        void MarkAsCompleted(int todoId, DateTime completitionDate, int userId);

        void Restore(int todoId, int userId);

        /// <summary>
        /// Pobierz kalendarz danego użytkownika
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ICollection<Day> GetCallendar(int userId);

        /// <summary>
        /// Usuń todo
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IServiceResult DeleteTodo(int userId, int todoId, bool force = false);
        IServiceResult<ICollection<EventModel>> GetEvents(int userId);
        IServiceResult Recalculate(int userId);

        /// <summary>
        /// Add tag to todo
        /// </summary>
        /// <param name="todoId"></param>
        /// <param name="tag"></param>
        Task AddTag(int todoId, string tag);
    }

    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly UsersContext _context;
        private readonly IMailingService _mailingService;
        private readonly IMailProvider _mailProvider;
        private readonly IEncryptor _encryptor;

        public UserService(
            IOptions<AppSettings> appSettings,
            UsersContext context, 
            IMailingService mailingService,
            IMailProvider mailProvider,
            IEncryptor encryptor)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _mailingService = mailingService;
            _mailProvider = mailProvider;
            _encryptor = encryptor;
        }

        public User Authenticate(string username, string password, DateTime dateTime)
        {
            var user = _context.GetUserForLogin(username, password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            

            user.AddDailyLoginReward(dateTime);

            _context.SaveChanges();
            user.Password = null;
            return user;
        }

        public async Task<IValidationState> RegisterUser(UserRegistrationModel registrationModel)
        {
            ValidationState validationState = new ValidationState();

            if (registrationModel == null)
            {
                validationState.AddError("Invalid data.", "Data");
                return validationState;
            }

            if (string.IsNullOrEmpty(registrationModel.Username))
            {
                validationState.AddError("Invalid username.", "Username");
                return validationState;
            }

            if (registrationModel.Password.Length < 6)
            {
                validationState.AddError("Invalid password.", "Password");
                return validationState;
            }

            var existingUser = _context.Users.SingleOrDefault(x => x.Username == registrationModel.Username);

            if (existingUser != null)
            {
                validationState.AddError("User already exists, choose another username.", "Username");
                return validationState;
            }

            var newUser = new User()
            {
                Username = registrationModel.Username,
                Regdate = DateTime.Now,
                Password = registrationModel.Password,
                Email = registrationModel.Email
            };

            var code = Guid.NewGuid().ToString();
            var registrationCode = HttpUtility.UrlEncode(_encryptor.Encrypt(JsonConvert.SerializeObject(new ActivationModel
            {
                Username = registrationModel.Username,
                GenerationDate = DateTime.Now,
                Password = registrationModel.Password,
                Code = code
            }, new JsonSerializerSettings()
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            })));

            newUser.AddCode(code);

            var mail =  await _mailProvider.GetRegistrationMailBody($"http://localhost:8080/activate/{registrationCode}");

            await _mailingService.SendEmail(mail, newUser.Email);

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return validationState;
        }

        public IEnumerable<object> GetAll()
        {
            // return users without passwords
            return _context.Users
                .Include(u => u.UserTodos);
        }

        public ICollection<Day> GetCallendar(int userId)
        {
            return _context.Users.Include(u => u.UserTodos).Single(x => x.Id == userId).Callendar;
        }

        public void AddTodo(TodoItem todoItem)
        {
            _context.SaveChanges();
        }

        /// <summary>
        /// Metoda dodaje nowe todo dla użytkownika
        /// </summary>
        /// <param name="description"></param>
        /// <param name="isCompleted"></param>
        /// <param name="name"></param>
        /// <param name="userId"></param>
        /// <param name="regdate"></param>
        /// <param name="targetDate"></param>
        public void AddTodo(string description, bool isCompleted, string name, int userId, DateTime regdate, DateTime targetDate)
        {
            var user = _context.Users.Include(u => u.UserTodos).Where(x => x.Id == userId).SingleOrDefault() ?? throw new Exception($"No user with id: {userId}");

            if (user.UserTodos.Where(x => x.TargetDate == targetDate).Count() + 1 > SystemSettings.MaxDayTodos)
                throw new Exception($"Cannot add more todos for date {targetDate}");

            if (targetDate.Date < DateTime.Now.Date)
                throw new Exception("Cannot add todo for date from the past");

            user.UserTodos.Add(new TodoItem()
            {
                Completed = false,
                Description = description,
                Name = name,
                Regdate = regdate,
                TargetDate = targetDate,
                Tags = new List<string>()
            });

            var count = _context.SaveChanges();
        }


        public void MarkAsCompleted(int todoId, DateTime completitionDate, int userId)
        {
            var user =_context.GetUserById(userId)?? throw new Exception($"No user with id: {userId}");

            var todo = user.UserTodos.SingleOrDefault(x => x.Id == todoId) ?? throw new Exception($"No todo with id {todoId} for user {user.Id} found");

            todo.Finish(completitionDate);
            user.AddTodoCompletedEvent(todo);

            _context.SaveChanges();
        }

        public void Restore(int todoId, int userId)
        {
            var user = _context.Users.Include(u => u.UserTodos).Where(x => x.Id == userId).SingleOrDefault() ?? throw new Exception($"No user with id: {userId}");

            var todo = user.UserTodos.SingleOrDefault(x => x.Id == todoId) ?? throw new Exception($"No todo with id {todoId} for user {user.Id} found");

            todo.Undo();

            _context.SaveChanges();
        }

        public IServiceResult DeleteTodo(int userId, int todoId, bool force = false)
        {
            ServiceResult serviceResult = new ServiceResult();
            var user = _context.Users.Include(x => x.UserTodos).SingleOrDefault(x => x.Id == userId);

            if (user == null)
            {
                serviceResult.ValidationState.AddError("User not found", "UserId");
                return serviceResult;
            }

            var todo = user.UserTodos.SingleOrDefault(x => x.Id == todoId);

            if (todo == null)
            {
                serviceResult.ValidationState.AddError("Todo not found", "Todo");
                return serviceResult;
            }

            var success = user.UserTodos.Remove(todo);

            if (!success)
            {
                serviceResult.ValidationState.AddError("Could not delete todo", "Todo");
                return serviceResult;
            }

            _context.SaveChanges();

            return serviceResult;
        }

        public IServiceResult<ICollection<EventModel>> GetEvents(int userId)
        {
            ServiceResult<ICollection<EventModel>> serviceResult = new ServiceResult<ICollection<EventModel>>();
            var user = _context.Users
                .Include(x => x.Events)
                    .ThenInclude(e => e.State)
                .SingleOrDefault(x => x.Id == userId);

            if (user == null)
            {
                serviceResult.ValidationState.AddError("User not found", "UserId");
                return serviceResult;
            }

            serviceResult.Result = MapEvents(user.Events);

            return serviceResult;
        }

        public IServiceResult Recalculate(int userId)
        {
            ServiceResult serviceResult = new ServiceResult();
            var user = _context.Users.Include(x => x.Events).SingleOrDefault(x => x.Id == userId);

            if (user == null)
            {
                serviceResult.ValidationState.AddError("User not found", "UserId");
                return serviceResult;
            }

            user.Calculate();

            _context.SaveChanges();

            return serviceResult;
        }

        private ICollection<EventModel> MapEvents(IEnumerable<BaseEvent> baseEvents)
        {
            List<EventModel> result = new List<EventModel>();

            foreach(var @event in baseEvents)
            {
                result.Add(new EventModel()
                {
                    EventDate = @event.Date,
                    EventType = nameMappings[@event.GetType()],
                    Points = @event?.Points ?? 0,
                    PointsAfterEvent = @event.State.Points,
                    Id = @event.Id
                });
            }

            return result;
        }

        public async Task AddTag(int todoId, string tag)
        {
            var todo =  await _context.Todos.SingleOrDefaultAsync(x => x.Id == todoId);

            if (todo == null)
                throw new Exception($"Could not find todo with id {todoId}");

            todo.Tags = todo.Tags.Concat(new string[] { tag });
        }

        /// <summary>
        /// Metoda służy do aktywowania konta użytkownika
        /// </summary>
        /// <param name="secret"></param>
        /// <returns></returns>
        public async Task<IValidationState> ActivateAccount(string secret)
        {
            try
            {
                var userJson = HttpUtility.UrlDecode(_encryptor.Decrypt(secret));

                if (userJson == null)
                    throw new Exception("Could not decrypt secret.");

                var format = "yyyy-MM-dd HH:mm:ss"; // your datetime format
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

                var activationModel = JsonConvert.DeserializeObject<ActivationModel>(userJson, dateTimeConverter);

                var user = _context.GetUserForLogin(activationModel.Username, activationModel.Password);

                user.Activate(activationModel.Code);

                return new ValidationState();

            }
            catch(Exception ex)
            {
                return new ValidationState()
                    .AddValidationError($"Error during acccount activation. {ex.Message}", "");
            }
        }

        private static Dictionary<Type, string> nameMappings = new Dictionary<Type, string>()
        {
            { typeof(DailyLoginEvent), "Daily login" },
            { typeof(TodoCompletedEvent), "Todo completed" },
            { typeof(WeeklyLoginEvent), "Weekly login" }
        };
    }
}
