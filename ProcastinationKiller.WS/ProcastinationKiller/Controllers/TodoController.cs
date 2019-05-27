using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcastinationKiller.Models;
using ProcastinationKiller.Models.Responses.Abstract;
using ProcastinationKiller.Services;

namespace ProcastinationKiller.Controllers
{
    [Produces("application/json")]
    [Route("api/Todo")]
    [Authorize]
    public class TodoController : Controller
    {

        //private readonly TodoContext _context;
        private readonly IUserService _userService;

        public TodoController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("AddTodo")]
        public async Task<ActionResult> AddItem([FromBody] TodoInputModel input)
        {
            try
            {
                _userService.AddTodo(input.Description, false, input.Name, input.UserId, DateTime.Now, input.TargetDate);

                return Ok();
            }
            catch(Exception ex)
            {
                return ValidationProblem();
            }
        }

        [HttpPost]
        [Route("MarkCompleted")]
        public async Task<ActionResult> MarkCompleted([FromBody] TodoCompleteInputModel input)
        {
            try
            {
                _userService.MarkAsCompleted(input.Id, DateTime.Now, input.UserId);

                return Ok();
            }
            catch (Exception ex)
            {
                return ValidationProblem();
            }
        }

        [HttpPost]
        [Route("Restore")]
        public async Task<ActionResult> Restore([FromBody] TodoCompleteInputModel input)
        {
            try
            {
                _userService.Restore(input.Id, input.UserId);

                return Ok();
            }
            catch (Exception ex)
            {
                return ValidationProblem();
            }
        }

        [HttpDelete]
        [Route("deleteTodo/{userId:int}/{todoId:int}")]
        public IServiceResult DeleteTodo(int userId, int todoId)
        {
            return _userService.DeleteTodo(userId, todoId);
        }
    }
}