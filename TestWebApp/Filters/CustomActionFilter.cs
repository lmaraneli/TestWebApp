using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace TestWebApp.Filters
{
    public class CustomActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Log.Information($"Action '{context.Controller}.{context.ActionDescriptor.DisplayName}' filter executed");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Log.Information($"Action '{context.Controller}.{context.ActionDescriptor.DisplayName}' filter executing");
        }
    }
}
