using ProcastinationKiller.Models.Responses.Abstract;

namespace ProcastinationKiller.Models.Responses
{
    public class ServiceResult<TResult> : IServiceResult<TResult>
    {
        public TResult Result { get; set; }
        public bool IsOk { get; set; }
        public StatusCode StatusCode { get; set; }
    }
}
