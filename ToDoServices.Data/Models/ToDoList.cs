using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ToDoServices.Data.Models
{
    public class ToDoList
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }

        public virtual List<ToDoItem> ToDoItems { get; set; }
    }
}