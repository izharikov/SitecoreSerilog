using log4net.Appender;
using log4net.spi;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using SitecoreSerilog.Extensions;
using ILogger = Serilog.ILogger;

namespace SitecoreSerilog.Appenders
{
    public abstract class BaseSitecoreSerilogAppender : BufferingAppenderSkeleton
    {
        public string? MinimumLevel { get; set; }
        private Logger? _serilogLogger;

        protected override void SendBuffer(LoggingEvent[] events)
        {
            if (_serilogLogger == null)
            {
                return;
            }

            foreach (var thisEvent in events)
            {
                LogEvent(_serilogLogger, thisEvent);
            }
        }

        protected override bool RequiresLayout => true;

        private void LogEvent(ILogger? log, LoggingEvent loggingEvent)
        {
            try
            {
                var message = RenderLoggingEvent(loggingEvent);
                var level = GetLogEventLevel(loggingEvent.Level.ToString());
                var exception = loggingEvent.GetException();
                log?.Write(level, exception, message);
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Error occurred while logging the event.", ex);
            }
        }

        private static LogEventLevel GetLogEventLevel(string? level, LogEventLevel defaultValue = LogEventLevel.Debug)
        {
            var logEventLevel = defaultValue;
            switch (level?.ToLowerInvariant())
            {
                case "debug":
                    logEventLevel = LogEventLevel.Debug;
                    break;
                case "info":
                    logEventLevel = LogEventLevel.Information;
                    break;
                case "warn":
                    logEventLevel = LogEventLevel.Warning;
                    break;
                case "error":
                    logEventLevel = LogEventLevel.Error;
                    break;
                case "fatal":
                    logEventLevel = LogEventLevel.Fatal;
                    break;
            }

            return logEventLevel;
        }
        
        public override void ActivateOptions()
        {
            base.ActivateOptions();
            if (!ValidateConfiguration())
            {
                return;
            }
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel
                .ControlledBy(new LoggingLevelSwitch(GetLogEventLevel(MinimumLevel, LogEventLevel.Information)))
                .Enrich.WithExceptionDetails()
                ;
            loggerConfig = Enrich(loggerConfig);
            loggerConfig = WriteTo(loggerConfig);
            _serilogLogger = loggerConfig.CreateLogger();
        }
        
        public override void OnClose()
        {
            base.OnClose();
            _serilogLogger?.Dispose();
        }

        protected abstract LoggerConfiguration Enrich(LoggerConfiguration configuration);
        protected abstract LoggerConfiguration WriteTo(LoggerConfiguration configuration);
        protected abstract bool ValidateConfiguration();
    }
}