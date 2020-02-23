using System.Data.Entity;

namespace ToDoServices.Data.Models
{
    public class ToDoListContext : DbContext
    {
        public ToDoListContext() : base("name=ToDoListContext")
        {
            Database.SetInitializer<ToDoListContext>(new DropCreateDatabaseIfModelChanges<ToDoListContext>());
        }
        public DbSet<ToDoServices.Data.Models.ToDoList> BoToDoLists { get; set; }
        public DbSet<ToDoServices.Data.Models.ToDoItem> ToDoItems { get; set; }

    }
}
