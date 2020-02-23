using System.ComponentModel.DataAnnotations;

namespace ToDoServices.Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
 
        public string Role { get; set; }
        
    }
}
