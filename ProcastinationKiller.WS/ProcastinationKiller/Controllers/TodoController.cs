using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcastinationKiller.Models;
using ProcastinationKiller.Services;

namespace ProcastinationKiller.Controllers
{
    [Produces("application/json")]
    [Route("api/Todo")]
    [Authorize]
    public class TodoController : Controller
    {

        private readonly TodoContext _context;
        private readonly IUserService _userService;

        public TodoController(TodoContext ctx, IUserService userService)
        {
            _context = ctx;
            _userService = userService;
        }

        [HttpGet]
        [Route("Get/{id:int}")]
        public async Task<ActionResult<TodoItem>> GetItem(int id)
        {
            var item = await _context.TodoItems.FindAsync(id);

            if (item == null)
                return NotFound();

            return item; 
        }

        [HttpGet]
        [Route("GetAll")]
        public TodoItem[] GetAll()
        {
            var items = _context.TodoItems.Select(x => x).ToArray();

            return items;
        }

        [HttpPost]
        [Route("AddTodo")]
        public async Task<ActionResult> AddItem([FromBody] TodoInputModel input)
        {
            try
            {
                var newItem = new TodoItem()
                {
                    Description = input.Description,
                    Completed = false,
                    Name = input.Name,
                    UserId = input.UserId,
                    Regdate = DateTime.Now
                };

                //_context.Add(newItem);
                //await _context.SaveChangesAsync();

                _userService.AddTodo(newItem);

                return Ok();
            }
            catch(Exception ex)
            {
                return ValidationProblem();
            }
        }
    }
}