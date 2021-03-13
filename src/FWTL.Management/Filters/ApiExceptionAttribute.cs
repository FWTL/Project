using System.Linq;
using FluentValidation;
using FWTL.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FWTL.Management.Filters
{
    public sealed class ApiExceptionAttribute : IExceptionFilter
    {
        private readonly IGuidService _guid;
        private readonly IWebHostEnvironment _hosting;

        private readonly ILogger _logger;
        private readonly string _source;

        public ApiExceptionAttribute(ILogger logger, IWebHostEnvironment hosting, IGuidService guid, string source)
        {
            _logger = logger;
            _hosting = hosting;
            _guid = guid;
            _source = source;
        }

        public  void OnException(ExceptionContext context)
        {
            if (context.Exception.InnerException is ValidationException exceptionInner)
            {
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new JsonResult(exceptionInner.Errors
                    .GroupBy(e => e.PropertyName.ToLower())
                    .ToDictionary(e => e.Key, e => e.Select(element => element.ErrorMessage).ToList()));
                return;
            }

            if (context.Exception is ValidationException exception)
            {
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new JsonResult(exception.Errors
                    .GroupBy(e => e.PropertyName.ToLower())
                    .ToDictionary(e => e.Key, e => e.Select(element => element.ErrorMessage).ToList()));
                return;
            }

            context.HttpContext.Response.StatusCode = 500;

            var exceptionId = _guid.New;
            //_logger.LogError(
            //    "ExceptionId: {exceptionId} {NewLine}" +
            //           "Url: {url} {NewLine}" +
            //           "Exception: {exception} {NewLine}" +
            //           "Source: {source}",
            //    exceptionId,
            //    context.HttpContext.Request.GetDisplayUrl(),
            //    context.Exception,
            //    _source);

            context.Result = _hosting.IsDevelopment() ? new ContentResult { Content = context.Exception.ToString() } : new ContentResult { Content = exceptionId.ToString() };
        }
    }
}