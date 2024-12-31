using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace web.features.shared;

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
                        context.Result = new BadRequestObjectResult(context.ModelState);
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
