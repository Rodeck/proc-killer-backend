using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcastinationKiller.Controllers.Abstract;
using ProcastinationKiller.Models;
using ProcastinationKiller.Models.Requests;
using ProcastinationKiller.Models.Responses;
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

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IServiceResult<User> Authenticate([FromBody]LoginModel userParam)
        {
            var user = _userService.Authenticate(userParam.Username, userParam.Password, DateTime.Now);

            if (user == null)
                return AuthenticationError<User>()
                    .AddValidationError("Username", "Username or password incorrect.");

            return Ok(user);
        }

        [EnableCors("any")]
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]UserRegistrationModel userParam)
        {
            var validation = await _userService.RegisterUser(userParam);

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

        [HttpGet]
        [Route("getEvents/{userId:int}")]
        public IServiceResult<ICollection<EventModel>> GetAllEvents(int userId)
        {
            var events = _userService.GetEvents(userId);
            return events;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("recalculateEvents/{userId:int}")]
        public IServiceResult RecalculateState(int userId)
        {
            return _userService.Recalculate(userId);
        }

        private static List<string> _urls = new List<string>()
        {
            "http://www.google.com",
            "http://www.facebook.com",
            "http://www.yahoo.com",
            "http://www.msdn.microsoft.com.com",
            "http://www.youtube.com",
            "http://www.reddit.com",
        };

        [HttpGet]
        [AllowAnonymous]
        [Route("FetchWebsites")]
        public IServiceResult<WebResult> Fetch()
        {
            var watch = new Stopwatch();
            watch.Start();
            var result = new List<int>();
            var client = new WebClient();

            foreach(var url in _urls)
            {
                result.Add(client.DownloadString(url).Length);
            }

            watch.Stop();

            return new ServiceResult<WebResult>()
            {
                Result = new WebResult()
                {
                    ContentLength = result,
                    Time = watch.ElapsedMilliseconds
                }
            };
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("FetchWebsitesAsync")]
        public async Task<IServiceResult<WebResult>> FetchAsync()
        {
            var watch = new Stopwatch();
            watch.Start();
            var result = new List<int>();
            var client = new WebClient();

            foreach (var url in _urls)
            {
                result.Add(await Task.Run(() => FetchUrl(url)));
            }

            watch.Stop();

            return new ServiceResult<WebResult>()
            {
                Result = new WebResult()
                {
                    ContentLength = result,
                    Time = watch.ElapsedMilliseconds
                }
            };
        }

        public async Task<int> FetchUrl(string url)
        {
            var client = new WebClient();
            return (await client.DownloadStringTaskAsync(url)).Length;
        }

        public class WebResult
        {
            public long Time { get; set; }

            public IEnumerable<int> ContentLength { get; set; }
        }
    }
}