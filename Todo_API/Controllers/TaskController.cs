using Microsoft.AspNetCore.Mvc;
using Todo_API.Models;

namespace Todo_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly ToDoDbContext _dbContext; // use the EXACT name Scaffold-DbContext generated

        public BookingsController(ToDoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //This will get all the tasks
        [HttpGet("getAll")] //header -> this is what the server looks at when creating the URL 
        public IActionResult GetAllTasks()
        {
            var tasks = _dbContext.TaskItems.ToList(); 
            return Ok(tasks);
        }

        // GET one specific task
        //api/task/5 

        [HttpGet("get/{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetTask(Guid id)
        {
            var task = _dbContext.TaskItems.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);//returns the tasks in json format also it shows what succeeded
        }

        [HttpPost("create")]
        public IActionResult CreateTask([FromBody] TaskItem newTask) //explicitly says that content will be sent from the request body
        {
            newTask.Id = Guid.NewGuid();
            _dbContext.TaskItems.Add(newTask);
            _dbContext.SaveChanges();

            return Ok(); //less information
            //Better status code to return in a post method
           // return CreatedAtAction(); //gives more information its more precise
        }

        /* Create the delete endpoint */

        // DELETE - remove a task
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteTask(Guid id)
        {
            var task = _dbContext.TaskItems.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            _dbContext.TaskItems.Remove(task);
            _dbContext.SaveChanges();

            return NoContent();
        }

        //PUT - update an existing task, needs an Id extension in the URL 
        [HttpPut("update/{id}")]
        public IActionResult UpdateTask(Guid id, [FromBody] bool isComplete)
        {
            var task = _dbContext.TaskItems.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }

             task.IsComplete = isComplete;
            _dbContext.SaveChanges();

            return Ok(task);
        }


    }
}
