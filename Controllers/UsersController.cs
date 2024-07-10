using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using newTask.Auth;
using newTask.Models;
using newTask.Packages;
using newTask.Validators;


namespace newTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : MainController
    {
        IPKG_USERS package;
        IPKG_ERROR_LOGS logs;

        private readonly IJwtManager jwtManager;

        public UsersController(IPKG_USERS package, IJwtManager jwtManager, IPKG_ERROR_LOGS logs)
        {
            this.package = package;
            this.jwtManager = jwtManager;
            this.logs = logs;
        }
        [HttpPost("Authenticate")]
        public IActionResult Authenticate(Login loginData)
        {
            Token? token = null;
            User? user = null;
           
                user = package.Authenticate(loginData);
                if (user == null) return Unauthorized("Invalid login credentials");

                token = jwtManager.GetToken(user);
           
            return Ok(token);
        }


        [HttpPost("addUser")]
        [Authorize(Roles ="admin")]
        public IActionResult AddUser([FromBody] User user)
        {
                if(UserValidator.Validate(user, out string errorMessage))
            {
                package.AddUser(user);
                return Ok(user);
            }

            return BadRequest(errorMessage);                    
        }


        [HttpPut("editUser/{userId}")]
        [Authorize(Roles = "admin")]

        public IActionResult EditUser([FromBody] User currentUser, int userId)
        {
            

                package.EditUser(currentUser, userId);
                return Ok();

           

        }
        [HttpGet("getUsers")]
        [Authorize(Roles = "admin, manager")]
        public IActionResult getUsers()
        {
            List<User> users = new List<User>();
            
                users = package.GetUsers();

            List<User> usersByCompany = users.Where(p => p.CompanyId == AuthUser.CompanyId).ToList();


            return Ok(usersByCompany);

        
           

        }


        [HttpGet("getAssignees")]
        [Authorize(Roles = "admin, manager")]
        public IActionResult getAssignees()
        {
            List<User> assignees = new List<User>();
          
                assignees = package.GetAssignees();

                List<User> assigneesByCompany = assignees.Where(p => p.CompanyId == AuthUser.CompanyId).ToList();


                return Ok(assigneesByCompany);

           

        }


        [HttpGet("getUser/{id}")]
        [Authorize(Roles = "admin, manager, developer")]
        public IActionResult getUser(int id)
        {

                User user = package.GetUser(id);
                if (user == null)
                {
                    return NotFound($"User with Id {id} not found.");
                }
                return Ok(user);
           

        }

        [HttpDelete("deleteUser/{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteUser(int id)
        {
           
                package.DeleteUser(id);
            
            return Ok();
        }

        [HttpPut("changePassword/{userId}")]
        [Authorize(Roles = "admin")]
        public IActionResult ChangePassword(int userId, User user)
        {
                package.ChangePassword(userId, user);
          
            return Ok();
        }

        [HttpGet("getReports")]
        [Authorize(Roles = "admin, manager")]

        public IActionResult getReports()
        {
            List<UserReport> reports = new List<UserReport>();
           
                reports = package.Report();

                return Ok(reports);


        }

      
    }
}