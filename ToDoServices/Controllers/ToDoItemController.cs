using System;
using System.Web.Http;
using System.Web.Http.Cors;
using ToDoServices.Data.Interfaces;
using ToDoServices.Data.Models;

namespace ToDoServices.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ToDoItemController : ApiController
    {
        private IToDoRepository _toDoRepository;

        public ToDoItemController(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }

        // ADD ITEM TO INDIVIDUAL LIST
        [HttpPost]
        [Route("api/todolists/{listId}/item")]
        public IHttpActionResult AddToDoItem(string userName, int listId, ToDoItem toDoItem)
        {

            // Check if a list item with provided Id already exists. If it does, we cannot create another list item with same Id.
            // Pull item from repository
            var existingItem = _toDoRepository.ToDoItemGet(toDoItem.ItemId);
            if (existingItem != null)
            {
                ModelState.AddModelError("", $"To Do Item with Id {toDoItem.ItemId} already exists!");
                return BadRequest(ModelState);
            }

            ToDoList result = null;
            try
            {
                 result = _toDoRepository.ToDoItemAdd(userName, listId, toDoItem);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            if (result != null)
            {
                return Ok(result);
            }
            return NotFound();
        }

        // REMOVE ITEM FROM INDIVIDUAL LIST
        [HttpDelete]
        [Route("api/todolists/{listId}/item/{itemId}")]
        public IHttpActionResult RemoveToDoItem(string userName, int listId, int itemId)
        {
            bool done;
            try
            {
                done = _toDoRepository.ToDoItemRemove(userName, listId, itemId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            if (done)
            {
                return Ok();
            }
            return NotFound();
        }

        // UPDATE ITEM
        [HttpPut]
        [Route("api/todolists/{listId}/item/{itemId}")]
        public IHttpActionResult UpdateToDoItem(string userName, int listId, ToDoItem toDoItem)
        {
            bool done;
            try
            {
                // if ListId was not set by caller, set it now.
                toDoItem.ToDoListId = toDoItem.ToDoListId ?? listId;
                done = _toDoRepository.ToDoItemUpdate(userName, listId, toDoItem);
                // Pull item from repository
                toDoItem = _toDoRepository.ToDoItemGet(toDoItem.ItemId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            if (done)
            {
                return Ok(toDoItem);
            }
            return NotFound();
        }

        // MARK ITEM AS COMPLETED
        [HttpPut]
        [Route("api/todolists/{listId}/item/{itemId}/done")]
        public IHttpActionResult MarkToDoItemAsCompleted(string userName, int listId, int itemId)
        {
            // To Do: Code needs error checking
            bool done;
            ToDoItem toDoItem;
            try
            {
                toDoItem = _toDoRepository.ToDoItemGet(itemId);
                toDoItem.Completed = true;
                toDoItem.CompletedDate = DateTime.UtcNow;
                done = _toDoRepository.ToDoItemUpdate(userName, (int)toDoItem.ToDoListId, toDoItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            if (done)
            {
                return Ok(toDoItem);
            }
            return NotFound();
        }
    }
}
