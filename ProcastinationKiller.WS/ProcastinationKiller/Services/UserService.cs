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
        IEnumerable<User> GetAll();

        /// <summary>
        /// Zarejestruj nowego użytkownika
        /// </summary>
        /// <param name="registrationModel"></param>
        /// <returns></returns>
        IValidationState RegisterUser(UserRegistrationModel registrationModel);

        void AddTodo(TodoItem todoItem);
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

        public IEnumerable<User> GetAll()
        {
            // return users without passwords
            return _context.Users;
        }

        public void AddTodo(TodoItem todoItem)
        {
            var user = _context.Users.SingleOrDefault(x => x.Id == todoItem.UserId)
                ?? throw new Exception($"No user with id: {todoItem.UserId}");

            if (user.UserTodos == null)
                user.UserTodos = new List<TodoItem>();

            user.UserTodos.Add(todoItem);

            _context.SaveChanges();
        }
    }
}
