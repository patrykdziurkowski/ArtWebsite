using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace web.Features.Shared;

public class ValidateModelFilter : IActionFilter
{
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
                if (context.ModelState.IsValid)
                {
                        return;
                }

                object? model = context.ActionArguments.Values.FirstOrDefault();
                if (model is null)
                {
                        List<string> errors = context.ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage).ToList();
                        string reason = string.Join("; ", errors);

                        context.Result = new ViewResult
                        {
                                ViewName = "Error",
                                ViewData = new ViewDataDictionary(
                                        new EmptyModelMetadataProvider(), context.ModelState)
                                {
                                        Model = new ErrorViewModel(400, reason)
                                }
                        };
                        return;
                }

                context.Result = new ViewResult
                {
                        ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState)
                        {
                                Model = model
                        }
                };
        }

}
