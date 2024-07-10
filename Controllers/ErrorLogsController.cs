using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using newTask.Models;
using newTask.Packages;
using newTask.Validators;

namespace newTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorLogsController : ControllerBase
    {
        IPKG_ERROR_LOGS package;

        public ErrorLogsController(IPKG_ERROR_LOGS package)
        {
            this.package = package;
        }

        [HttpPost("addLog")]
        public IActionResult SaveLogs(string ErrorText, int? UserId)
        {

            try
            {
                package.SaveLogs(ErrorText, UserId);
                return Ok();
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                return StatusCode(StatusCodes.Status500InternalServerError, "System error, try again: " + ex.Message + " Inner Exception: " + innerMessage);
            }

        }



        //[HttpGet("getLogs")]
        //public IActionResult GetLogs()
        //{
        //    List<Registration> companies = package.GetAll();
        //    try

        //    {
        //        if (companies == null || companies.Count == 0)
        //        {
        //            return NotFound("No companies found.");
        //        }
        //        return Ok(companies);
        //    }
        //    catch (Exception ex)
        //    {
        //        var innerMessage = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
        //        return StatusCode(StatusCodes.Status500InternalServerError, "System error, try again: " + ex.Message + " Inner Exception: " + innerMessage);
        //    }

        //}

    }
}
