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
using Microsoft.Extensions.Logging;
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
        private ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]UserRegistrationModel userParam)
        {
            _logger.LogInformation($"Register user {userParam.Email}");
            var validation = await _userService.RegisterUser(GetUserId(), userParam);

            if (!validation.Success())
                return BadRequest(validation);

            return Ok();
        }

        [HttpGet]
        [Route("getCallendar")]
        public ICollection<Day> GetCallendar()
        {
            return _userService.GetCallendar(GetUserId());
        }

        [HttpPost]
        [Route("getDay")]
        public Task<Day> GetDay([FromBody] LoadTodoModel loadTodo)
        {
            return _userService.GetDay(GetUserId(), loadTodo.Date);
        }

        [HttpGet]
        [Route("getEvents")]
        public ICollection<EventModel> GetAllEvents()
        {
            return _userService.GetEvents(GetUserId());
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("recalculateEvents/{userId:int}")]
        public Task<IServiceResult> RecalculateState(string userId)
        {
            return _userService.Recalculate(userId);
        }
    }
}