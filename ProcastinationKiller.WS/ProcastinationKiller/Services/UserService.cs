using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProcastinationKiller.Helpers;
using ProcastinationKiller.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
        User Authenticate(string username, string password);

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
        IValidationState RegisterUser(UserRegistrationModel registrationModel);

        void AddTodo(string description, bool isCompleted, string name, int userId, DateTime regdate, DateTime targetDate);

        void MarkAsCompleted(int todoId, DateTime completitionDate, int userId);

        /// <summary>
        /// Pobierz kalendarz danego użytkownika
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ICollection<Day> GetCallendar(int userId);
    }

    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly UsersContext _context;

        public UserService(IOptions<AppSettings> appSettings, UsersContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }

        public User Authenticate(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(x => x.Username == username && x.Password == password);

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
            user.Password = null;

            return user;
        }

        public IValidationState RegisterUser(UserRegistrationModel registrationModel)
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
                Password = registrationModel.Password
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return validationState;
        }

        public IEnumerable<object> GetAll()
        {
            // return users without passwords
            return _context.Users
                .Include(u => u.UserTodos)
                /*
                .Select(u => new
                {
                    UserName = u.Username,
                    Todos = u.UserTodos.Select(t => new
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        Completed = t.Completed,
                        FinishTime = t.FinishTime,
                        TargetDate = t.TargetDate
                    }),
                    Regdate = u.Regdate,
                    Callendar = u.Callendar
                }
                )}*/;
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
            var user = _context.Find<User>(userId) ?? throw new Exception($"No user with id: {userId}");

            if (user.UserTodos.Where(x => x.TargetDate == targetDate).Count() + 1 >= SystemSettings.MaxDayTodos)
                throw new Exception($"Cannot add more todos for date {targetDate}");

            if (targetDate.Date < DateTime.Now.Date)
                throw new Exception("Cannot add todo for date from the past");

            user.UserTodos.Add(new TodoItem()
            {
                Completed = false,
                Description = description,
                Name = name,
                Regdate = regdate,
                TargetDate = targetDate
            });

            var count = _context.SaveChanges();
        }


        public void MarkAsCompleted(int todoId, DateTime completitionDate, int userId)
        {
            var user = _context.Find<User>(userId) ?? throw new Exception($"No user with id: {userId}");

            var todo = user.UserTodos.SingleOrDefault(x => x.Id == todoId) ?? throw new Exception($"No todo with id {todoId} for user {user.Id} found");

            todo.Finish(completitionDate);

        }
    }
}
