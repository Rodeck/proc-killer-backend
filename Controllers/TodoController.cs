﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcastinationKiller.Controllers.Abstract;
using ProcastinationKiller.Models;
using ProcastinationKiller.Models.Responses.Abstract;
using ProcastinationKiller.Services;

namespace ProcastinationKiller.Controllers
{
    [Produces("application/json")]
    [Route("api/Todo")]
    [Authorize]
    public class TodoController : BaseController
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
                _userService.AddTodo(input.Description, false, input.Name, GetUserId(), DateTime.Now, input.TargetDate);

                return Ok();
            }
            catch(Exception ex)
            {
                return ValidationProblem();
            }
        }


        [HttpPost]
        [Route("AddTodoForFriend")]
        public Task AddItemFromFriend([FromBody] TodoFromFriendInputModel input)
        {
            return _userService.AddTodoFromFriend(input.Description, false, input.Name, input.TargetId, DateTime.Now, input.TargetDate, GetUserId());
        }

        [HttpPost]
        [Route("AddTag")]
        public async Task<ActionResult> AddTag(int todoId, string tag)
        {
            try
            {
                await _userService.AddTag(todoId, tag);

                return Ok();
            }
            catch (Exception ex)
            {
                return ValidationProblem();
            }
        }

        [HttpPost]
        [Route("MarkCompleted")]
        public Task MarkCompleted([FromQuery] int id)
        {
            return _userService.MarkAsCompleted(id, DateTime.Now, GetUserId());
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

        [HttpGet]
        [Route("loadUnfinished")]
        public Task<IEnumerable<TodoItem>> Unfinished()
        {
            return _userService.GetUnfinished(GetUserId());
        }

        [HttpPost]
        [Route("completeUnfinished")]
        public Task CompleteUnfinished([FromQuery] int todoId)
        {
            return _userService.CompleteUnfinished(GetUserId(), todoId);
        }

        [HttpDelete]
        [Route("deleteTodo/{userId:int}/{todoId:int}")]
        public IServiceResult DeleteTodo(string userId, int todoId)
        {
            return _userService.DeleteTodo(userId, todoId);
        }
    }
}