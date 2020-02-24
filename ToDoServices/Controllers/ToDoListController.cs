using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using ToDoServices.Data.Interfaces;
using ToDoServices.Data.Models;
using ToDoServices.Interfaces;

namespace ToDoServices.Controllers
{
    //change port number. The below url is the GUI url
    //[EnableCors(origins: "http://localhost:56473", headers: "*", methods: "*")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ToDoListController : ApiController
    {
        #region Initializares       
        private readonly IToDoRepository _toDoRepository;
        private readonly IAuthorization _authorization;

        public ToDoListController(IToDoRepository toDoRepository, IAuthorization authorization)
        {
            _toDoRepository = toDoRepository;
            _authorization = authorization;
        }
        #endregion

        #region API Calls
       
        // CREATE TO DO LIST
        [HttpPost]
        [Route("api/todolists")]
        public IHttpActionResult ToDoListAdd(string userName, ToDoList toDoList)
        {
            // Any user can create To Do List. No authorization call nedded

            // Check if ToDoList has all required fields set
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if a list with provided Id exist. If it does, we cannot create another list with same Id.
            // Also we cannot add another list with same Title.
            string match;
            if (_toDoRepository.ToDoListExists(userName, toDoList.Title, toDoList.Id, out match))
            {
                ModelState.AddModelError("", $"To Do List with {match} already exists!");
                return BadRequest(ModelState);
            }

            // Create To Do List in repository
            var isCreated = CreateToDoList(toDoList, out string error);

            // If errors occured during call to repository, return them to caller and stop execution of the call
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }

            if (isCreated)
            {
                return CreatedAtRoute("ToDoListGet", new {userName = userName, id = toDoList.Id }, toDoList);
            }

            // Someting went wrong
            return NotFound();
        }

        // RETRIEVE ALL TO DO LISTS
        [HttpGet]
        [Route("api/todolists")]
        public IHttpActionResult ToDoListGetAll(string userName)
        {
            // Any user can pull To Do List. No authorization call nedded.

            IEnumerable<ToDoList> list;

            try
            {
                // Pull All To Do Lists from repo. 
                // For users other than Admin filter on Author.
                list = _toDoRepository.ToDoListGetAll(userName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }        

            return Ok(list);
        }

        // RETRIEVE SPECIFIC TO DO LIST
        [HttpGet]
        // GET api/todolist/5
        [Route("api/todolists/{id:int}", Name = "ToDoListGet")]
        public IHttpActionResult ToDoListGet(string userName, int id)
        {
            // Check if list exists in repository
            var existingList = GetToDoList((int)id, out string error);

            // If errors occured during call to repository, return them to caller and stop execution of the call
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }

            // If list does not exist, we cannot pull it
            // stop execution pf the call and return error message
            if (existingList == null)
            {
                return BadRequest($"No To Do list with ID {id}");
            }

            // Check if user is Authorized to update To Do List
            // If user is authorized to update, he or she is also authorized to view it
            if (!IsAuthorized(userName, existingList, out error))
            {
                if (!string.IsNullOrEmpty(error))
                {
                    return BadRequest(error);
                }
                return Unauthorized();
            }

            return Ok(existingList);
        }

        // UPDATE TO DO LIST
        [HttpPut]
        [Route("api/todolists/{id:int}")]    
        public IHttpActionResult UpdateToDoList(int id, string userName, ToDoList toDoList)
        {
            // Check if ToDoList has all required fields set
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          
            // Check if list exists in repository
            var existingList = GetToDoList((int)id, out string error);

            // If errors occured during call to repository, return them to caller and stop execution of the call
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }

            // If list does not exist, we cannot update it
            // stop execution pf the call and return error message
            if (existingList == null)
            {
                return BadRequest($"No To Do list with ID {id}");
            }

            // Check if user is Authorized to Update To Do List
            if (!IsAuthorized(userName, existingList, out error))
            {
                if (!string.IsNullOrEmpty(error))
                {
                    return BadRequest(error);
                }
                return Unauthorized();
            }


            // Update existing To Do List in repository
            var isUpdated = UpdateToDoList(id, toDoList, out error);

            // If errors occured during call to repository, return them to caller and stop execution of the call
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }

            if (isUpdated)
            {
                // Pull updated list from repository
                var updatedList = GetToDoList((int)id, out error);
                // If errors occured during call to repository, return them to caller and stop execution of the call
                if (!string.IsNullOrEmpty(error))
                {
                    return BadRequest(error);
                }
                return Ok(_toDoRepository.ToDoListGet(id));
            }

            // Someting went wrong
            return NotFound();
        }

        // DELETE TO DO LIST
        [HttpDelete]
        [Route("api/todolists/{id:int}")]
        public IHttpActionResult DeleteToDoList(int? id, string userName)
        {
            if (id == null)
            {
                return BadRequest($"List ID is missing");
            }

            var toDoList = _toDoRepository.ToDoListGet((int)id);

            if (toDoList == null)
            {
                return BadRequest($"No To Do list with ID {id}");
            }

            if (!_authorization.IsAuthorizedToDelete(userName, toDoList))
            {
                return Unauthorized();
            }

            bool done;
            try
            {
                done = _toDoRepository.ToDoListRemove((int)id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            if (done)
            {
                return Ok(_toDoRepository.ToDoListGetAll(userName));
            }

            return NotFound();
        }

        // ADD ITEMS TO A TO DO LIST
        [HttpPut]
        [Route("api/todolists/{id:int}/todoi tems")]
        public IHttpActionResult ToDoListAddItems(int id, List<ToDoItem> toDoItems)
        {    
            var list = _toDoRepository.ToDoListAddItems(id, toDoItems);
            if (list != null)
            {
                return Ok(list);
            }
           
            return NotFound();
        }
        #endregion

        #region Helpers
        private ToDoList GetToDoList(int listId, out string error)
        {
            error = string.Empty;
            ToDoList list = null;
            try
            {
                list = _toDoRepository.ToDoListGet(listId);
            }
            catch(Exception ex)
            {
                error = ex.Message;
            }

            return list;
        }

        private bool CreateToDoList(ToDoList toDoList, out string error)
        {
            error = string.Empty;
            bool done = false ;

            try
            {
                done = _toDoRepository.ToDoListAdd(toDoList);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return done;
        }

        private bool UpdateToDoList(int listId, ToDoList list, out string error)
        {
            error = string.Empty;
            bool done = false; ;
            try
            {
                done = _toDoRepository.ToDoListUpdate(listId, list);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return done;
        }

        private bool IsAuthorized(string userName, ToDoList list, out string error)
        {
            error = string.Empty;
            bool isAuthorized = false; ;
            try
            {
                isAuthorized = _authorization.IsAuthorizedToUpdate(userName, list);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return isAuthorized;
        }
        #endregion
    }
}
