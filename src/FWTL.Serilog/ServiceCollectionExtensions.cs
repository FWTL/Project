using System;
using Serilog;
using Serilog.Events;

namespace FWTL.Serilog
{
    public static class ServiceCollectionExtensions
    {
        public static LoggerConfiguration AddSerilog(this ILogger logger)
        {
            const string format =
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {NewLine}{Message:lj}{NewLine}{Exception}";

            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .WriteTo.Console(outputTemplate: format)
                .Enrich.FromLogContext();
        }

        public static LoggerConfiguration AddSeq(this LoggerConfiguration loggerConfiguration, Uri seqUrl)
        {
            return loggerConfiguration.WriteTo.Seq(seqUrl.AbsoluteUri);
        }
    }
}
