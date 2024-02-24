using Application.Common.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace WebApi.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDateTimeService _dateTimeService;

        public ErrorHandlingMiddleware(RequestDelegate next, IDateTimeService dateTimeService)
        {
            _next = next;
            _dateTimeService = dateTimeService;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static string ReturnBadRequestResponse(ResponseException error)
        {
            var errorResponse = JsonConvert.SerializeObject(new
            {
                error.StatusCode,
                error.Succeeded,
                error.Message,
                error.Errors,
                error.Data
            });

            return errorResponse;
        }


        private string ReturnInternalServerResponse(Exception error)
        {
            if (error is null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            var logError = JsonConvert.SerializeObject(new
            {
                ErrorMessage = error.Message,
                StackTrace = error.Demystify(),
                InnerException = error.InnerException == null ? "" : error.InnerException.Message,
                DateTime = _dateTimeService.Now.ToString("g")
            });

            Log.Error(logError, "[SERVER ERROR]");

            var errorResponse = JsonConvert.SerializeObject(new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "The API has encountered an Internal Server Error. Please try again.",
                ErrorDateTime = _dateTimeService.Now.ToString("g")
            });

            return errorResponse;
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            switch (ex)
            {
                case ResponseException e:
                    context.Response.StatusCode = e.StatusCode;
                    await context.Response.WriteAsync(ReturnBadRequestResponse(e));
                    break;
                case Exception e:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsync(ReturnInternalServerResponse(e));
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsync(ReturnInternalServerResponse(ex));
                    break;
            }

        }
    }
}