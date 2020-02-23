using System.Collections.Generic;
using ToDoServices.Data.Models;

namespace ToDoServices.Data.Interfaces
{
    public interface IToDoRepository
    {
        //CRUD
        //Create
        bool ToDoListAdd(ToDoList toDoList);
        //Retrieve
        ToDoList ToDoListGet(int id);
        //Update
        bool ToDoListUpdate(int id, ToDoList toDoList);
        //Delete
        bool ToDoListRemove(int id);

        List<ToDoList> ToDoListGetAll(string userName, bool listAllForAdmin = true);
        bool ToDoListExists(string userName, string title, int id, out string match);

        bool IsOwner(string userName, ToDoList toDoList);

        ToDoList ToDoListAddItems(int id, List<ToDoItem> toDoItems);
       
        ToDoList ToDoItemAdd(string userName, int id, ToDoItem toDoItem);
        bool ToDoItemRemove(string userName, int listId, int itemId);
        bool ToDoItemUpdate(string userName, int listId, ToDoItem toDoItem);
        ToDoItem ToDoItemGet(int id);
    }
}
