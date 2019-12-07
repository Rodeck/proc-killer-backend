using Microsoft.AspNetCore.Mvc;
using ProcastinationKiller.Models;
using ProcastinationKiller.Models.Responses;
using ProcastinationKiller.Models.Responses.Abstract;
using System;
using System.Linq;

namespace ProcastinationKiller.Controllers.Abstract
{
    public abstract class BaseController: ControllerBase
    {
        protected string GetUserId() {
            return HttpContext.User.Identities.First().Claims.Single(x => x.Type == "user_id").Value;
        }

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
                IsOk = false,
                ValidationState = new ValidationState(),
                StatusCode = ProcastinationKiller.Models.Responses.Abstract.StatusCode.InternalServerError
            };
        }

        public virtual IServiceResult<TResult> AuthenticationError<TResult>()
        {
            return new ServiceResult<TResult>()
            {
                IsOk = false,
                ValidationState = new ValidationState(),
                StatusCode = ProcastinationKiller.Models.Responses.Abstract.StatusCode.AuthenticationError
            };
        }
    }
}
