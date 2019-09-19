using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public interface IValidationState
    {
        void AddError(string msg, string @object);

        bool Success();
    }

    public class ValidationError
    {
        public string Object { get; set; }

        public string Message { get; set; }
    }

    public class ValidationState : IValidationState
    {
        public bool Success = true;
        public IEnumerable<ValidationError> Errors = new List<ValidationError>();

        public void AddError(string msg, string @object)
        {
            Errors = Errors.Concat(new ValidationError[] { new ValidationError() { Message = msg, Object = @object } });
            Success = false;
        }

        bool IValidationState.Success()
        {
            return Success;
        }
    }

    public static class ValidationExtensions
    {
        public static ValidationState AddValidationError(this ValidationState validationState, string msg, string @object)
        {
            validationState.AddError(msg, @object);
            return validationState;
        }
    }
}
