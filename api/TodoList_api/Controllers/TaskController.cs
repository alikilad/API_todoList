using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todolist_API.Models;
using todolist_API.Data;

namespace todolist_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TaskController : ControllerBase
    {
        //Injecting DataContext via constructor
        private readonly DataContext _dbContext; 

        public TaskController(DataContext dbContext)
        {
            _dbContext = dbContext; // Assign the injected dbContext to the private field
        }

        //Retrieve all tasks:
        [HttpGet]
        public IActionResult GetAllTasks()
        {
            var allTasks = _dbContext.Tasks.Include(t => t.Tags).ToList(); //Returns all tasks including their associated tags
            return Ok(allTasks); //Returns 200
        }

        //Retrieve a specific task by id:
        [HttpGet("{id}")]
        public IActionResult GetTaskById(int id)
        {
            var task = _dbContext.Tasks.Find(id);

            if (task == null)
            {
                return NotFound(); // Returns 404
            }
            
            return Ok(task); //Returns task if found
        }

       
        // Create a new task:
        [HttpPost]
        public IActionResult CreateTask([FromBody] todolist_API.Models.Task task)   
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // Return 400 Bad Request if the model state is invalid
            }

            _dbContext.Tasks.Add(task);  // Add the new task to the DbContext
            _dbContext.SaveChanges();  // Save the changes to the database

            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);  // Return 201 Created
        }

        // Update an existing task:
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] todolist_API.Models.Task task)
        {
            if (id != task.Id)
            {
                return BadRequest(); // Return 400 Bad Request if the task ID in the URL does not match the body
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return 400 Bad Request if the model state is invalid
            }

            _dbContext.Entry(task).State = EntityState.Modified; // Mark the task entity as modified
            _dbContext.SaveChanges(); // Save the changes to the database

            return NoContent(); // Return 204 No Content on successful update
        }

        // Delete a task:
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var task = _dbContext.Tasks.Find(id); // Find the task by id

            if (task == null)
            {
                return NotFound(); // Return 404 Not Found if the task does not exist
            }

            _dbContext.Tasks.Remove(task); // Remove the task from the DbContext
            _dbContext.SaveChanges(); // Save the changes to the database

            return NoContent(); // Return 204 No Content on successful deletion
        }
    }
}