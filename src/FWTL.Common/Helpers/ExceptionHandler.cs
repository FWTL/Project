using System;
using FWTL.Core.Helpers;
using FWTL.Core.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FWTL.Common.Helpers
{
    public class ExceptionHandler : IExceptionHandler
    {
        public ExceptionHandler(IGuidService guidService, ILogger<ExceptionHandler> logger)
        {
            _guidService = guidService;
            _logger = logger;
        }

        private readonly IGuidService _guidService;
        private readonly ILogger<ExceptionHandler> _logger;

        public Guid Handle<T>(Exception exception, T body)
        {
            var exceptionId = _guidService.New;
            string json = JsonConvert.SerializeObject(body);
            string source = body.GetType().FullName;

            _logger.LogError(
                "ExceptionId: {exceptionId} Body: {json} Exception: {exception} Source: {source}",
                exceptionId,
                json,
                exception,
                source);

            return exceptionId;
        }
    }
}