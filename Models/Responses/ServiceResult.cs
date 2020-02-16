using ProcastinationKiller.Models.Responses.Abstract;

namespace ProcastinationKiller.Models.Responses
{
    public class ServiceResult<TResult> : ServiceResult, IServiceResult<TResult>
    {
        public TResult Result { get; set; }
    }

    public class ServiceResult: IServiceResult
    {
        public bool IsOk { get; set; }
        public StatusCode StatusCode { get; set; }
        public IValidationState ValidationState { get; set; }

        public ServiceResult()
        {
            ValidationState = new ValidationState();
        }

        public void AddError(string message, string @object)
        {
            ValidationState.AddError(message, @object);
        }
    }

    public static class ServiceResultExtensions
    {
        public static IServiceResult<TResult> AddValidationError<TResult>(this IServiceResult<TResult> result, string message, string @object)
        {
            result.AddError(message, @object);
            return result;
        }
    }
}
