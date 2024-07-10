using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using newTask.Auth;
using newTask.Models;
using newTask.Packages;
using newTask.Validators;
using Task = newTask.Models.Task;
using TaskStatus = newTask.Models.TaskStatus;

namespace newTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : MainController
    {
        IPKG_ERROR_LOGS logs;

        private readonly IJwtManager jwtManager;
        IPKG_TASKS package;

        public TasksController(IPKG_TASKS package, IJwtManager jwtManager, IPKG_ERROR_LOGS logs)
        {
            this.package = package;
            this.jwtManager = jwtManager;
            this.logs = logs;
        }

        [HttpPost("addTask")]
        [Authorize(Roles = "manager")]
        public IActionResult AddTask([FromBody] Task task)
        {
            if (TaskValidator.Validate(task, out string errorMessage))
            {
                package.AddTask(task);

                return Ok(task);
            }

            return BadRequest(errorMessage);

        }

        [HttpPut("editTask/{taskId}")]
        [Authorize(Roles = "manager")]
        public IActionResult EditTask([FromBody] Task task, int taskId)
        {

            if (taskId != task.Id)
            {
                return BadRequest("Task ID mismatch");
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token");
            }

            int currentUserId = int.Parse(userIdClaim.Value);

            var currentTask = package.GetTask(taskId);
            if (currentTask == null)
            {
                return NotFound("Task not found");
            }

            if (currentTask.OwnerId != currentUserId)
            {
                return Forbid("You are not allowed to edit this task");
            }

            package.EditTask(task, taskId);

            return Ok(task);
        }



        [HttpPut("completeTask/{taskId}")]
        [Authorize(Roles = "developer")]
        public IActionResult CompleteTask([FromBody] TaskStatusUpdateDto dto)
        {
            //int x = (int)dto.Status;
            package.CompleteTask(dto.TaskId, dto.Status);

            return Ok();
        }


        [HttpDelete("deleteTask/{taskId}")]
        [Authorize(Roles = "manager")]
        public IActionResult DeleteTask(int taskId)
        {

            package.DeleteTask(taskId);

            return Ok();
        }



        [HttpPut("startTask/{taskId}")]
        [Authorize(Roles = "developer")]
        public IActionResult StartTask([FromBody] TaskStatusUpdateDto dto)
        {

            package.StartTask(dto.TaskId, dto.Status);


            return Ok();
        }


        [HttpGet("getTasks/{companyId}")]
        [Authorize(Roles = "admin, manager, developer")]
        public IActionResult getTasks(int companyId)
        {
            List<Task> tasks = new List<Task>();


            tasks = package.GetTasks(companyId);

            //if (tasks.Count == 0)
            //{
            //    return Ok("No Tasks found");
            //}
            return Ok(tasks);


        }


        [HttpGet("tasksByAssignee/{assigneeId}")]
        [Authorize(Roles = "admin, manager, developer")]
        public IActionResult TasksByAssignee(int assigneeId)
        {
            List<Task> tasks = new List<Task>();

            tasks = package.GetTasksByAssignee(assigneeId);
            if (!tasks.Any())
            {
                return Ok("No tasks found");
            }
            return Ok(tasks);

        }

        [HttpGet("tasksByOwner/{ownerId}")]
        [Authorize(Roles = "admin, manager, developer")]
        public IActionResult TasksByOwner(int ownerId)
        {
            List<Task> tasks = new List<Task>();

            tasks = package.GetTasksByOwner(ownerId);
            if (!tasks.Any())
            {
                return Ok("No users found");
            }
            return Ok(tasks);

        }

        [HttpGet("getTask/{taskId}")]
        [Authorize(Roles = "admin, manager, developer")]
        public IActionResult getTask(int taskId)
        {
            Task task = null;

            task = package.GetTask(taskId);

            return Ok(task);


        }

        [HttpGet("tasksCompletedByUser/{assigneeId}")]
        [Authorize(Roles = "admin, manager, developer")]
        public IActionResult TasksCompletedByUser(int assigneeId)
        {
            List<Task> tasks = null;

            tasks = package.TasksCompletedByUser(assigneeId);
            //if(!tasks.Any())
            //{
            //    return Ok("No tasks found");
            //}

            return Ok(tasks);


        }


        [HttpGet("tasksCompleted/{companyId}")]
        [Authorize(Roles = "admin, manager, developer")]
        public IActionResult TasksCompleted(int companyId)
        {
            List<Task> tasks = null;

            tasks = package.TasksCompleted(companyId);
            if (!tasks.Any())
            {
                return Ok("No tasks found");
            }

            return Ok(tasks);



        }


        [HttpGet("tasksNew/{companyId}")]
        [Authorize(Roles = "admin, manager, developer")]
        public IActionResult TasksNew(int companyId)
        {
            List<Task> tasks = null;

            tasks = package.TasksNew(companyId);
            if (!tasks.Any())
            {
                return Ok("No tasks found");
            }

            return Ok(tasks);


        }

        [HttpGet("tasksInProgressByUser/{assigneeId}")]
        [Authorize(Roles = "admin, manager, developer")]
        public IActionResult TasksInProgressByUser(int assigneeId)
        {
            List<Task> tasks = null;

            tasks = package.TasksInProgressByUser(assigneeId);
            if (!tasks.Any())
            {
                return Ok("No tasks found");
            }

            return Ok(tasks);

        }

        [HttpGet("tasksInProgress/{companyId}")]
        [Authorize(Roles = "manager, developer")]
        public IActionResult TasksInProgress(int companyId)
        {
            List<Task> tasks = null;

            tasks = package.TasksInProgress(companyId);
            if (!tasks.Any())
            {
                return Ok("No tasks found");
            }

            return Ok(tasks);
        }

        [HttpGet("tasksOverdueByUser/{assigneeId}")]
        [Authorize(Roles = "admin, manager, developer")]
        public IActionResult TasksOverdueByUser(int assigneeId)
        {
            List<Task> tasks = null;

            tasks = package.TasksOverdueByUser(assigneeId);
            if (!tasks.Any())
            {
                return Ok("No tasks found");
            }

            return Ok(tasks);



        }


        [HttpGet("tasksOverdue/{companyId}")]
        [Authorize(Roles = "admin, manager, developer")]
        public IActionResult TasksOverdue(int companyId)
        {
            List<Task> tasks = null;

            tasks = package.TasksOverdue(companyId);
            if (!tasks.Any())
            {
                return Ok("No tasks found");
            }

            return Ok(tasks);


        }

    }
}
