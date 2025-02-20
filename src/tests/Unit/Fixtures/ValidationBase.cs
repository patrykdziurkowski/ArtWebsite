using System.ComponentModel.DataAnnotations;

namespace tests.Unit.Fixtures;

public abstract class ValidationBase
{
        protected IList<ValidationResult> Validate(object model)
        {
                var validationResults = new List<ValidationResult>();
                var ctx = new ValidationContext(model, null, null);
                Validator.TryValidateObject(model, ctx, validationResults, true);
                return validationResults;
        }
}
