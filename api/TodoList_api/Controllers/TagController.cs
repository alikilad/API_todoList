using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todolist_API.Models;
using todolist_API.Data;

namespace todolist_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TagController : ControllerBase
    {
        private readonly DataContext _dbContext; 

        public TagController(DataContext dbContext)
        {
            _dbContext = dbContext; // Assign the injected dbContext to the private field
        }

        //Retrieve all tags
        [HttpGet]
        public IActionResult GetAllTags() 
        {
            var allTags = _dbContext.Tags.Include(t => t.Tasks).ToList();
            return Ok(allTags); //Return OK response
        }
        // Create a new tag:
        [HttpPost]
        public IActionResult CreateTag([FromBody] Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return 400 Bad Request if the model state is invalid
            }

            _dbContext.Tags.Add(tag); // Add the new tag to the DbContext
            _dbContext.SaveChanges(); // Save the changes to the database

            return CreatedAtAction("GetTagById", new { id = tag.Id }, tag); // Return 201 Created
        }
        // Update an existing tag:
        [HttpPut("{id}")]
        public IActionResult UpdateTag(int id, [FromBody] Tag tag)
        {
            if (id != tag.Id)
            {
                return BadRequest(); // Return 400 Bad Request if the tag ID in the URL does not match the body
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return 400 Bad Request if the model state is invalid
            }

            _dbContext.Entry(tag).State = EntityState.Modified; // Mark the tag entity as modified
            _dbContext.SaveChanges(); // Save the changes to the database

            return NoContent(); // Return 204 No Content on successful update
        }
        // Get all tasks for a specific tag:
        [HttpGet("{id}/tasks")]
        public IActionResult GetTasksForTag(int id)
        {
            var tag = _dbContext.Tags.Include(t => t.Tasks).FirstOrDefault(t => t.Id == id); // Eagerly load tasks

            if (tag == null)
            {
                return NotFound(); // Return 404 Not Found if the tag does not exist
            }

            return Ok(tag.Tasks); // Return 200 OK with the tasks for the tag
        }

        // Delete a tag:
        [HttpDelete("{id}")]
        public IActionResult DeleteTag(int id)
        {
            var tag = _dbContext.Tags.Find(id); // Find the tag by id

            if (tag == null)
            {
                return NotFound(); // Return 404 Not Found if the tag does not exist
            }

            _dbContext.Tags.Remove(tag); // Remove the tag from the DbContext
            _dbContext.SaveChanges(); // Save the changes to the database

            return NoContent(); // Return 204 No Content on successful deletion
        }
        // Associate tags with a task:
        [HttpPost("{id}/tags")]
        public IActionResult AssociateTagsWithTask(int id, [FromBody] List<int> tagIds)
        {
            var task = _dbContext.Tasks.Include(t => t.Tags).FirstOrDefault(t => t.Id == id); // Eagerly load tags for the task

            if (task == null)
            {
                return NotFound(); // Return 404 Not Found if the task does not exist
            }

            // Get the tags by their IDs
            var tags = _dbContext.Tags.Where(t => tagIds.Contains(t.Id)).ToList();

            // Associate the tags with the task
            task.Tags.AddRange(tags);

            _dbContext.SaveChanges(); // Save the changes to the database

            return NoContent(); // Return 204 No Content on successful association
        }

    }
}