using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RaspSecure.Models.Exceptions;
using System;
using System.Net;

namespace RaspSecure.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var (statusCode, errorCode) = context.Exception.ParseException();

            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = (int)statusCode;
            context.Result = new JsonResult(new
            {
                error = context.Exception.Message,
                code = errorCode
            });
        }
    }
    public static class CustomExceptionFilter
    {
        public static (HttpStatusCode statusCode, ErrorCode errorCode) ParseException(this Exception exception)
        {
            switch (exception)
            {
                case NotFoundException _:
                    return (HttpStatusCode.NotFound, ErrorCode.NotFound);
                case InvalidUsernameOrPasswordException _:
                    return (HttpStatusCode.Unauthorized, ErrorCode.InvalidUsernameOrPassword);
                case InvalidTokenException _:
                    return (HttpStatusCode.Unauthorized, ErrorCode.InvalidToken);
                case ExpiredRefreshTokenException _:
                    return (HttpStatusCode.Unauthorized, ErrorCode.ExpiredRefreshToken);
                default:
                    return (HttpStatusCode.InternalServerError, ErrorCode.General);
            }
        }
    }
}
