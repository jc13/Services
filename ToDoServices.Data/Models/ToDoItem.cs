using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoServices.Data.Models
{
    public class ToDoItem
    {
        [Key]
        public int ItemId { get; set; }
        // [ForeignKey("ToDoList")]
        public int? ToDoListId { get; set; }
        [Required]
        public string ToDoStep { get; set; }
        public string Description { get; set; }
        public byte Priority { get; set; }
        public bool Completed { get; set; }
        public DateTime CompletedDate { get; set; }
    }
}
