using Microsoft.AspNetCore.Mvc;
using ProcastinationKiller.Models;
using ProcastinationKiller.Models.Responses;
using ProcastinationKiller.Models.Responses.Abstract;
using System;

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

        public virtual IServiceResult<TResult> Error<TResult>(Exception ex)
        {
            return new ServiceResult<TResult>()
            {
                IsOk = true,
                ValidationState = new ValidationState(),
                StatusCode = ProcastinationKiller.Models.Responses.Abstract.StatusCode.InternalServerError
            };
        }
    }
}
