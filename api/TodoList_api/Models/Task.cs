namespace todolist_API.Models
{
    public class Task
    {
        public int Id { get; set;}
        public string Name { get; set;} = string.Empty;
        
        // Many-to-Many relationship with tag
        
        public List<Tag> Tags { get; set;} = new List<Tag>();

    }
}
