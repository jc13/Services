using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToDoServices.Data.Models;

namespace ToDoServices.Interfaces
{
    public interface IAuthorization
    {
        bool IsAuthorizedToDelete(string userName, ToDoList list);
        bool IsAuthorizedToUpdate(string userName, ToDoList list);
        bool IsAuthorizedToDelete(string userName, ToDoItem item);
        bool IsAuthorizedToUpdate(string userName, ToDoItem item);
    }
}