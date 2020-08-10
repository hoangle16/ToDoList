using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using ToDo.Domain.Entities;
using ToDo.Domain.Helpers;
using ToDo.Domain.Interfaces;
using ToDo.Domain.Models.ToDoItem;

namespace ToDo.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoItemsController : ControllerBase
    {
        private IToDoItemRepository _ToDoItemService;
        private IMapper _mapper;
        public ToDoItemsController(
            IToDoItemRepository toDoItemRepository,
            IMapper mapper)
        {
            _mapper = mapper;
            _ToDoItemService = toDoItemRepository;
        }

        [HttpPost]
        public IActionResult Add([FromBody] AddItemModel model)
        {
            var currentUserId = int.Parse(User.Identity.Name);
            var item = _mapper.Map<ToDoItem>(model);
            item.UserId = currentUserId;
            try
            {
                _ToDoItemService.Add(item);
                return Ok();
            }
            catch(AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("complete/{id}")]
        public IActionResult Complete(int id)
        {
            try
            {
                _ToDoItemService.Complete(id);
                return Ok();
            }
            catch(AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_ToDoItemService.GetAll());
        }

        //get item by id
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var currentUserId = int.Parse(User.Identity.Name);
            var item = _ToDoItemService.GetById(id);
            if (!User.IsInRole(Role.Admin) && item.UserId != currentUserId)
                return Forbid();

            if (item == null)
                return NotFound();
            var model = _mapper.Map<ItemModel>(item);
            return Ok(model);
        }

        //get all items of user by userId
        [HttpGet("user/{id}")]
        public IActionResult GetAllItemsOfUser(int id)
        {
            var currentUserId = int.Parse(User.Identity.Name);
            if (currentUserId != id && !User.IsInRole(Role.Admin))
                return Forbid();

            var items = _ToDoItemService.GetAllItemOfUser(id);
            if (items == null)
                throw new AppException("User not found");
            var model = _mapper.Map<IList<ItemModel>>(items);
            return Ok(model);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItem(int id)
        {
            var currentId = int.Parse(User.Identity.Name);
            var userId = _ToDoItemService.GetUserId(id);
            if (currentId != userId && !User.IsInRole(Role.Admin))
                return Forbid();

            _ToDoItemService.Remove(id);
            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateItem(int id, [FromBody] UpdateItemModel model)
        {
            var currentId = int.Parse(User.Identity.Name);
            var userId = _ToDoItemService.GetUserId(id);
            if (currentId != userId && !User.IsInRole(Role.Admin))
                return Forbid();

            var item = _mapper.Map<ToDoItem>(model);
            item.ToDoItemId = id;
            try
            {
                _ToDoItemService.Update(item);
                return Ok();
            }
            catch(AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}