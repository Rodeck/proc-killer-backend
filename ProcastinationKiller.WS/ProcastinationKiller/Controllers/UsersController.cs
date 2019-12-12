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

        [HttpGet]
        [Route("getUsers")]
        public IEnumerable<UserViewModel> GetAllUsers()
        {
            return _userService.GetAll(GetUserId());
        }

        [HttpGet]
        [Route("getFriends")]
        public IEnumerable<UserViewModel> GetFriends()
        {
            return _userService.GetFriends(GetUserId());
        }

        [HttpGet]
        [Route("getState")]
        public Task<UserState> GetState()
        {
            return _userService.GetUserState(GetUserId());
        }

        [HttpPost]
        [Route("authenticate")]
        public Task Authenticate()
        {
            return _userService.Authenticate(GetUserId());
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

        [HttpGet]
        [Route("getInvitations")]
        public Task<IEnumerable<InvitationViewModel>> GetInviations()
        {
            return _userService.GetInvitations(GetUserId());
        }

        [HttpGet]
        [Route("getUserDetails/{userId}")]
        public Task<UserDetailsViewModel> GetUserDetails(string userId)
        {
            return _userService.GetUserDetails(GetUserId(), userId);
        }

        [HttpPost]
        [Route("addInvitation")]
        public Task<int> AddInviation([FromBody] InviteDTO invited)
        {
            return _userService.InviteFriend(GetUserId(), invited.InvitedId);
        }

        [HttpPost]
        [Route("acceptInvitation")]
        public Task<int> AcceptInviation([FromBody] InvitationDTO invitationDTO)
        {
            return _userService.AcceptInvitation(GetUserId(), invitationDTO.InviterId, invitationDTO.InvitationId);
        }

        [HttpPost]
        [Route("rejectInvitation")]
        public Task<int> RejectInviation([FromBody] InvitationDTO invitationDTO)
        {
            return _userService.RejectInvitation(GetUserId(), invitationDTO.InviterId, invitationDTO.InvitationId);
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