using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models.Responses.Abstract
{
    public interface IServiceResult
    {
        bool IsOk { get; set; }

        StatusCode StatusCode { get; set; }

        IValidationState ValidationState { get; set; }

        void AddError(string message, string @object);
    }

    public interface IServiceResult<TResult>: IServiceResult
    {
        TResult Result { get; set; }
    }

    public enum StatusCode
    {
        Ok = 200,
        InternalServerError = 500,
        AuthenticationError = 401,
    }
}
