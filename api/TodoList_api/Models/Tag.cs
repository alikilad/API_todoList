
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations;
namespace todolist_API.Models
{
    public class Tag
    {
        public int Id { get; set;}
        public string Name { get; set;} = string.Empty;
        
        // Many-to-Many relationship with task
        
        public List<Task> Tasks { get; set;} = new List<Task>();

    }
}
