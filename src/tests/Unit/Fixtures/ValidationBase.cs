using System.ComponentModel.DataAnnotations;

namespace tests.Unit.Fixtures;

public abstract class ValidationBase
{
        protected IList<ValidationResult> Validate(object model)
        {
                List<ValidationResult> validationResults = [];
                ValidationContext context = new(model);
                Validator.TryValidateObject(model, context, validationResults,
                        validateAllProperties: true);
                return validationResults;
        }
}
