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
        /// Pobierz wszystkich użytkiowników
        /// </summary>
        /// <returns></returns>
        IEnumerable<UserViewModel> GetAll(string uid);

        IEnumerable<UserViewModel> GetFriends(string uid);

        /// <summary>
        /// Pobierz konkretny dzień
        /// </summary>
        /// <returns></returns>
        Task<Day> GetDay(string uid, DateTime date);

        /// <summary>
        /// Zarejestruj nowego użytkownika
        /// </summary>
        /// <param name="registrationModel"></param>
        /// <returns></returns>
        Task<IValidationState> RegisterUser(string uid, UserRegistrationModel registrationModel);

        void AddTodo(string description, bool isCompleted, string name, string userId, DateTime regdate, DateTime targetDate);

        Task MarkAsCompleted(int todoId, DateTime completitionDate, string userId);

        void Restore(int todoId, string userId);

        /// <summary>
        /// Pobierz kalendarz danego użytkownika
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ICollection<Day> GetCallendar(string userId);

        /// <summary>
        /// Usuń todo
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IServiceResult DeleteTodo(string userId, int todoId, bool force = false);

        ICollection<EventModel> GetEvents(string userId);

        Task<IServiceResult> Recalculate(string userId);

        /// <summary>
        /// Add tag to todo
        /// </summary>
        /// <param name="todoId"></param>
        /// <param name="tag"></param>
        Task AddTag(int todoId, string tag);

        Task<IEnumerable<TodoItem>> GetUnfinished(string userId);

        Task CompleteUnfinished(string userId, int todoId);
        
        Task Authenticate(string userId);

        Task<UserState> GetUserState(string userId);
    }

    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly UsersContext _context;
        private readonly IMailingService _mailingService;
        private readonly IMailProvider _mailProvider;
        private readonly IEncryptor _encryptor;
        private readonly IRewardService _rewardService;

        public UserService(
            IOptions<AppSettings> appSettings,
            UsersContext context, 
            IMailingService mailingService,
            IMailProvider mailProvider,
            IEncryptor encryptor,
            IRewardService rewardService)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _mailingService = mailingService;
            _mailProvider = mailProvider;
            _encryptor = encryptor;
            _rewardService = rewardService;
        }

        public async Task<IValidationState> RegisterUser(string uid, UserRegistrationModel registrationModel)
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

            var existingUser = _context.Users.SingleOrDefault(x => x.Username == registrationModel.Username);

            if (existingUser != null)
            {
                validationState.AddError("User already exists, choose another username.", "Username");
                return validationState;
            }

            var newUser = new User()
            {
                UId = uid,
                Username = registrationModel.Username,
                Regdate = DateTime.Now,
                Email = registrationModel.Email
            };

            _context.Users.Add(newUser);

            _context.SaveChanges();
            await _rewardService.AssignBaseRewards(newUser.UId);

            return validationState;
        }

        public IEnumerable<UserViewModel> GetAll(string uid)
        {
            var user = _context
                .Users
                .Include(x => x.Friends)
                .Single(x => x.UId == uid);
            
            var allUsers = _context
                .Users
                .Include(x => x.CurrentState)
                    .ThenInclude(x => x.Level)
                .Where(x => x.UId != uid);

            return allUsers.Select(u => new UserViewModel()
            {
                IsFriend = user.Friends.Any(y => y.FriendId == u.UId),
                Name = u.Username,
                Uid = u.UId,
                Level = u.CurrentState.Level.Number,
                Points = u.CurrentState.Points
            });
        }

        public ICollection<Day> GetCallendar(string userId)
        {
            return _context.Users.Include(u => u.UserTodos).Single(x => x.UId == userId).Callendar;
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
        public void AddTodo(string description, bool isCompleted, string name, string userId, DateTime regdate, DateTime targetDate)
        {
            var user = _context.Users.Include(u => u.UserTodos).Where(x => x.UId == userId).SingleOrDefault() ?? throw new Exception($"No user with id: {userId}");

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


        public async Task MarkAsCompleted(int todoId, DateTime completitionDate, string userId)
        {
            var user = _context.GetUserById(userId)?? throw new Exception($"No user with id: {userId}");

            var todo = user.UserTodos.SingleOrDefault(x => x.Id == todoId) ?? throw new Exception($"No todo with id {todoId} for user {user.Id} found");

            todo.Finish(completitionDate);
            user.AddTodoCompletedEvent(todo);
            await _rewardService.Calculate(userId, user.CurrentState.DailyLogins, user.CurrentState.Points, user.CurrentState.TotalTodosCompleted);

            _context.SaveChanges();
        }

        public void Restore(int todoId, string userId)
        {
            var user = _context.Users
                .Include(u => u.UserTodos)
                .Include(u => u.Events)
                .Where(x => x.UId == userId)
                .SingleOrDefault() ?? throw new Exception($"No user with id: {userId}");

            var todo = user.UserTodos.SingleOrDefault(x => x.Id == todoId) ?? throw new Exception($"No todo with id {todoId} for user {user.Id} found");

            var @event = user.Events.OfType<TodoCompletedEvent>().Single(x => x.CompletedItem == todo && !x.Hidden);
            user.HideEvent(@event.Id);

            todo.Undo();

            _context.SaveChanges();
        }

        public IServiceResult DeleteTodo(string userId, int todoId, bool force = false)
        {
            ServiceResult serviceResult = new ServiceResult();
            var user = _context.Users.Include(x => x.UserTodos).SingleOrDefault(x => x.UId == userId);

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

        public ICollection<EventModel> GetEvents(string userId)
        {
            var user = _context.Users
                .Include(x => x.Events)
                    .ThenInclude(e => e.State)
                .SingleOrDefault(x => x.UId == userId);

            return MapEvents(user.Events);
        }

        public async Task<IServiceResult> Recalculate(string userId)
        {
            ServiceResult serviceResult = new ServiceResult();
            var user = _context.Users.Include(x => x.Events).SingleOrDefault(x => x.UId == userId);

            if (user == null)
            {
                serviceResult.ValidationState.AddError("User not found", "UserId");
                return serviceResult;
            }

            user.Calculate();
            await _rewardService.Calculate(userId, user.CurrentState.DailyLogins, user.CurrentState.Points, user.CurrentState.TotalTodosCompleted);

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

        public Task<Day> GetDay(string uid, DateTime date)
        {
            return Task.FromResult(_context.Users.Include(u => u.UserTodos).Single(x => x.UId == uid).Callendar.Single(x => x.Date == date));
        }

        public async Task<IEnumerable<TodoItem>> GetUnfinished(string userId)
        {
            var user = await _context
                .Users
                .Include(x => x.UserTodos)
                .SingleAsync(x => x.UId == userId);

            return user.UserTodos.Where(x => !x.Completed && x.TargetDate.Date != DateTime.Now.Date).OrderByDescending(x => x.TargetDate);
        }

        public async Task CompleteUnfinished(string userId, int todoId)
        {
            var user = await _context
                .Users
                .Include(x => x.UserTodos)
                .SingleAsync(x => x.UId == userId);

            var todo = user.UserTodos.Single(x => x.Id == todoId);

            todo.Finish(DateTime.Now);

            await _context.SaveChangesAsync();
        }

        public Task Authenticate(string userId)
        {
            var user = _context.GetUserForLogin(userId);
            user.AddDailyLoginReward(DateTime.Now);
            return _context.SaveChangesAsync();
        }

        public Task<UserState> GetUserState(string userId)
        {
           var user = _context.GetUserForLogin(userId);
           return Task.FromResult(user.CurrentState);
        }

        public IEnumerable<UserViewModel> GetFriends(string uid)
        {
            var user = _context
                .Users
                .Include(x => x.Friends)
                .Single(x => x.UId == uid);
            
            return user.Friends.Select(f => new UserViewModel()
            {
                Name = f.FriendName,
                IsFriend = true,
                Uid = f.FriendId,
            });
        }

        private static Dictionary<Type, string> nameMappings = new Dictionary<Type, string>()
        {
            { typeof(DailyLoginEvent), "Daily login" },
            { typeof(TodoCompletedEvent), "Todo completed" },
            { typeof(WeeklyLoginEvent), "Weekly login" }
        };
    }
}
