using ToDoServices.Data.Interfaces;
using ToDoServices.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoServices.Data.Repositories
{
    public class BookDatabase : IToDoRepository
    {
        private LibraryContext db = new LibraryContext();

        public ToDoList AddToDoItem(int id, ToDoItem toDoItem)
        {
            ToDoList book = GetToDoList(id);
            book.ToDoItem = toDoItem;
            db.SaveChanges();
            return book;
        }

        public bool AddNewToDoList(ToDoList book)
        {
            db.Books.Add(book);
            db.SaveChanges();
            return true;
        }

        public List<ToDoList> GetAllToDoLists()
        {
            return db.Books.ToList();
        }

        public string GetAuthorById(int id)
        {
            return db.Books.FirstOrDefault(x => x.Id == id).Author;
        }

        public ToDoList GetToDoList(int id)
        {
            return db.Books.FirstOrDefault(x => x.Id == id);
        }



        public List<ToDoList> GetBooksByAuthor(string name)
        {
            return db.Books.Where(x => x.Author.Contains(name)).ToList();
        }

        public bool Remove(int id)
        {
            var book = GetToDoList(id);
            if (book == null)
            {
                return false;
            }
            db.Books.Remove(book);
            db.SaveChanges();
            return true;
        }

        public List<ToDoList> UpdateToDoList(int id, ToDoList toDoList)
        {
            if (this.Remove(id))
            {
                this.AddNewToDoList(toDoList);
                db.SaveChanges();
                return db.Books.ToList();
            }
            return db.Books.ToList();
        }
    }
}
