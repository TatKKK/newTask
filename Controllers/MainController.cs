using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using newTask.Models;
using System.Security.Claims;

namespace newTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        protected User? AuthUser
        {
            get
            {
                User? user = null;
                var currentUser = HttpContext.User;

                if (currentUser != null && currentUser.HasClaim(c => c.Type == "UserId"))
                {
                    user = new User
                    {
                        UserId = int.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value),
                        Username = currentUser.Claims.FirstOrDefault(c => c.Type == "Username")?.Value,
                        CompanyId = int.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value),
                        Role = new Role
                        {
                            Name = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
                        }
                    };

                    if (currentUser.HasClaim(c => c.Type == "RoleId"))
                    {
                        user.RoleId = int.Parse(currentUser.Claims.FirstOrDefault(c => c.Type == "RoleId")?.Value);
                    }
                }
                return user;
            }
        }
    }
}
