using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace PostsApp.API.Configuration;

public static class LoggerConfiguration
{
    private const string LogTemplate =
        "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";

    public static void AddSerilogConfiguration(this ILoggingBuilder logging)
    {
		var configuration = new Serilog.LoggerConfiguration()
			.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
			.WriteTo.Console(LogEventLevel.Information, LogTemplate);
		var logger = configuration.CreateLogger();
        logging.AddSerilog(logger);
    }

}
