using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToDoServices.Data.Interfaces;
using ToDoServices.Data.Models;
using ToDoServices.Data.Repositories;
using ToDoServices.Interfaces;

namespace ToDoServices.Providers
{
    public class Authorization : IAuthorization
    {
        private readonly IUserRepository _userRepo;
        private readonly IToDoRepository _toDoRepo;

        public Authorization(IUserRepository userRepo, IToDoRepository toDoRepo)
        {
            _userRepo = userRepo;
            _toDoRepo = toDoRepo;
        }

        #region Interface Iplementation   

        public bool IsAuthorizedToDelete(string userName, ToDoList list)
        {
            var appUser = AppUser(userName);
            if (appUser.IsAdmin()) return true;

            return appUser.IsOwner(list);
        }

        public bool IsAuthorizedToDelete(string userName, ToDoItem item)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorizedToUpdate(string userName, ToDoList list)
        {
            return IsAuthorizedToDelete(userName, list);
        }

        public bool IsAuthorizedToUpdate(string userName, ToDoItem item)
        {
            return IsAuthorizedToDelete(userName, item);
        }

        #endregion

        #region Helpers
        private User AppUser(string userName)
        {
            return _userRepo.GetUser(userName);
        }
        private bool IsAdmin(string userName)
        {
            return AppUser(userName).IsAdmin();
        }
        #endregion
    }
}