using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using newTask.Packages;
using System.Net;

namespace newTask.FIlters
{
    public class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        readonly IPKG_ERROR_LOGS logs;

        public GlobalExceptionFilter(IPKG_ERROR_LOGS logs)
        {
            this.logs = logs;
        }

        public override void OnException(ExceptionContext context)
        {
            var result = new ObjectResult("System Error Try Again")
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            var error = context.Exception.Message;
            int? userId = GetUserIdFromContext(context);

            try
            {
                logs.SaveLogs(error, userId);
            }
            catch (Exception ex)
            {
                logs.SaveLogs(error, userId);
            }
            context.Result = result;
        }

        private int? GetUserIdFromContext(ExceptionContext context)
        {

            var userIdClaim = context.HttpContext.User?.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}