using System;
using System.Collections.Generic;
using System.Linq;
using ToDoServices.Data.Cache;
using ToDoServices.Data.Interfaces;
using ToDoServices.Data.Models;

namespace ToDoServices.Data.Repositories
{
    public class ToDoRepository : IToDoRepository
    {
        #region Initializers      
        private readonly ICacheService _cache;
        private readonly IUserRepository _userRepo;
        
        public ToDoRepository(ICacheService cache, IUserRepository userRepo)
        {
            _cache = cache;
            _userRepo = userRepo;
        }
        #endregion

        #region To Do List

        
        public List<ToDoList> ToDoLists {
            get
            {
                return _cache.GetOrSet(Config.CacheKey.ToDoListGetAll, () =>
                   new List<ToDoList>()
                   {
                        new ToDoList { Id = 1, Title = "List 1", Author = "Brian"},
                        new ToDoList { Id = 2, Title = "List 2", Author = "John"},
                        new ToDoList { Id = 3, Title = "List 3", Author = "Marry"},
                        new ToDoList { Id = 4, Title = "List 4", Author = "Brian"},
                        new ToDoList { Id = 5, Title = "List 5", Author = "Jerzy"},
                        new ToDoList { Id = 6, Title = "List 6", Author = "Brian"},
                   });
            }
    }

        public bool ToDoListAdd(ToDoList toDoList)
        {
            ToDoLists.Add(toDoList);
            _cache.Set(Config.CacheKey.ToDoListGetAll, () => ToDoLists);
            return true;
        }

        public bool IsOwner(string userName, ToDoList toDoList)
        {
            var u = _userRepo.GetUser(userName);
            return u.IsOwner(toDoList);
        }
        
        public List<ToDoList> ToDoListGetAll(string userName, bool listAllForAdmin = true)
        {
            var u = _userRepo.GetUser(userName);

            // if user is not specified, return an empty list
            if (u == null) return new List<ToDoList>();

            // if user logged in as admin and listAllForAdmin flag is set, return lists for all users
            if (u.IsAdmin() && listAllForAdmin) return ToDoLists;

            // if user is not ann admin, return only this user's lists
            return ToDoLists.Where(x => x.Author.Equals(userName, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        public ToDoList ToDoListGet(int id)
        {
            var toDoList = ToDoLists.FirstOrDefault(x => x.Id == id);
            return toDoList;
        }

         public bool ToDoListRemove(int id)
        {
            var toDoList = ToDoListGet(id);
            if (toDoList == null)
            {
                return false;
            }
            ToDoLists.Remove(toDoList);
            _cache.Set(Config.CacheKey.ToDoListGetAll, () => ToDoLists);

            return true;
        }

        public bool ToDoListUpdate(int id, ToDoList toDoList)
        {
            if (ToDoListRemove(id))
            {
                ToDoListAdd(toDoList);
                _cache.Set(Config.CacheKey.ToDoListGetAll, () => ToDoLists);
                return true;
            }
            return false;
        }

        public ToDoList ToDoListAddItems(int id, List<ToDoItem> toDoItems)
        {
            var toDoList = ToDoListGet(id);

            if(toDoList!=null)
            {
                if (toDoList.ToDoItems == null)
                {
                    toDoList.ToDoItems = new List<ToDoItem>();
                }

                toDoList.ToDoItems.AddRange(toDoItems);
                ToDoListUpdate(id, toDoList);
            }

            return toDoList;
        }

        public bool ToDoListExists(string userName, string title, int id, out string match)
        {
            match = string.Empty;
            var toDoLists = ToDoListGetAll(userName);

            var existing = toDoLists.SingleOrDefault((x => x.Id == id));
            //If list with specified Id exists, we cannot create another list with same Id
            if (existing != null)
            {
                match += $"Id = {id}";
                return true;
            }

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title must not be empty", "title");

            // Get all lists for specified userName.
            // Retrieve only Admin's lists for Admin.
            // We want to allow Admin to create a list with same Title as some other user.
            // Unique Key: Author, Title
            toDoLists = toDoLists.Where(x => x.Author.Equals(userName, StringComparison.InvariantCultureIgnoreCase)).ToList();

            existing = toDoLists.FirstOrDefault(x => 
                (x.Title.Equals(title, StringComparison.InvariantCultureIgnoreCase)));

            if (existing != null)
            {
                match += $"Title = [{title}]";
                return true;
            }
            return existing != null;
        }

        #endregion

        #region To Do List Item
       
        public ToDoList ToDoItemAdd(string userName, int listId, ToDoItem toDoItem)
        {
            var toDoList = ToDoListGet(listId);
            if (toDoList == null) throw new ArgumentException($"List with Id [{listId}] does not exist.");

            if (toDoList.ToDoItems == null) toDoList.ToDoItems = new List<ToDoItem>();

            toDoItem.ToDoListId = listId;
            toDoList.ToDoItems.Add(toDoItem);

            return toDoList;
        }
        public bool ToDoItemRemove(string userName, int listId, int itemId)
        {
            var toDoList = ToDoListGet(listId);
            if (toDoList == null) throw new ArgumentException($"List with Id [{listId}] does not exist.");

            ToDoItem toDoItem = null;
            if (toDoList.ToDoItems != null)
            {
                toDoItem = toDoList.ToDoItems.SingleOrDefault(x => x.ItemId == itemId);
            }

            if (toDoItem == null) throw new ArgumentException($"Item with Id [{itemId}]  does not exist in list with Id [{listId}].");

            toDoList.ToDoItems.Remove(toDoItem);

            return true;
        }
        public bool ToDoItemUpdate(string userName, int listId, ToDoItem toDoItem)
        {
            var toDoList = ToDoListGet(listId);
            if (toDoList == null) throw new ArgumentException($"List with Id [{listId}] does not exist.");

            ToDoItem existingItem = null;
            if (toDoList.ToDoItems != null)
            {
                existingItem = toDoList.ToDoItems.SingleOrDefault(x => x.ItemId == toDoItem.ItemId);
            }

            if (existingItem == null) throw new ArgumentException($"Item with Id [{toDoItem.ItemId}]  does not exist in list with Id [{listId}].");

            toDoList.ToDoItems.Remove(existingItem);
            toDoList.ToDoItems.Add(toDoItem);

            return true;
        }

        public ToDoItem ToDoItemGet(int itemId)
        {
            var matchingLists = ToDoLists
                .Where(list => 
                       list.ToDoItems != null 
                    && list.ToDoItems.SingleOrDefault(item => item.ItemId == itemId) != null)
                .Select(x => x);
                
            if (matchingLists == null || matchingLists.Count() != 1) return null;

            return matchingLists.Single().ToDoItems.Single();
        }
        #endregion


    }
}
