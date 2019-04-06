using Microsoft.AspNetCore.Mvc;
using ProcastinationKiller.Models.Responses;
using ProcastinationKiller.Models.Responses.Abstract;


namespace ProcastinationKiller.Controllers.Abstract
{
    public abstract class BaseController: ControllerBase
    {
        public virtual IServiceResult<TResult> Ok<TResult>(TResult result)
        {
            return new ServiceResult<TResult>()
            {
                IsOk = true,
                Result = result,
                StatusCode = ProcastinationKiller.Models.Responses.Abstract.StatusCode.Ok
            };
        }
    }
}
