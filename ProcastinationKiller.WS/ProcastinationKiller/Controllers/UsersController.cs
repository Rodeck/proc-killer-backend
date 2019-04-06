using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcastinationKiller.Controllers.Abstract;
using ProcastinationKiller.Models;
using ProcastinationKiller.Models.Requests;
using ProcastinationKiller.Models.Responses.Abstract;
using ProcastinationKiller.Services;

namespace ProcastinationKiller.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        private IUserService _userService;
        private UsersContext _usersContext;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IServiceResult<User> Authenticate([FromBody]LoginModel userParam)
        {
            var user = _userService.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
                return null;
                //return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register([FromBody]UserRegistrationModel userParam)
        {
            var validation = _userService.RegisterUser(userParam);

            if (!validation.Success())
                return BadRequest(validation);

            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public IServiceResult<object[]> GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users.ToArray());
        }

        [HttpGet]
        [Route("getCallendar/{userId:int}")]
        public IServiceResult<ICollection<Day>> GetCallendar(int userId)
        {
            var users = _userService.GetCallendar(userId);
            return Ok(users); 
        }
    }
}