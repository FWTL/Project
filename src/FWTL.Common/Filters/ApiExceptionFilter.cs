using System.IO;
using System.Linq;
using FWTL.Core.Services;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Microsoft.Extensions.Hosting;

namespace FWTL.Common.Filters
{
    public sealed class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IGuidService _guid;
        private readonly string _source;
        private readonly IWebHostEnvironment _hosting;

        private readonly ILogger _logger;

        public ApiExceptionFilter(ILogger logger, IWebHostEnvironment hosting, IGuidService guid, string source)
        {
            _logger = logger;
            _hosting = hosting;
            _guid = guid;
            _source = source;
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception.InnerException is ValidationException exceptionInner)
            {
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new JsonResult(exceptionInner.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(e => e.Key, e => e.Select(element => element.ErrorMessage).ToList()));
                return;
            }

            if (context.Exception is ValidationException exception)
            {
                context.HttpContext.Response.StatusCode = 400;
                context.Result = new JsonResult(exception.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(e => e.Key, e => e.Select(element => element.ErrorMessage).ToList()));
                return;
            }

            context.HttpContext.Response.StatusCode = 500;

            string body = string.Empty;
            if (context.HttpContext.Request.Body.CanSeek)
            {
                context.HttpContext.Request.Body.Position = 0;
                using (var reader = new StreamReader(context.HttpContext.Request.Body))
                {
                    body = reader.ReadToEnd();
                }
            }

            var exceptionId = _guid.New;
            _logger.Error("ExceptionId: {exceptionId} Url: {url} Body: {body} Exception: {exception} Source: {source}",
                exceptionId,
                context.HttpContext.Request.GetDisplayUrl(),
                body,
                context.Exception,
                _source);

            if (_hosting.IsDevelopment())
            {
                context.Result = new ContentResult() { Content = context.Exception.ToString() };
            }
            else
            {
                context.Result = new ContentResult() { Content = exceptionId.ToString() };
            }
        }
    }
}