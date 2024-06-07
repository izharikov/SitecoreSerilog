using log4net.Appender;
using log4net.spi;
using Serilog;
using Serilog.Core;
using Serilog.Events;
#if !NET452
using Serilog.Exceptions;
#endif
using SitecoreSerilog.Extensions;

namespace SitecoreSerilog.Appenders
{
    public abstract class BaseSitecoreSerilogAppender : AppenderSkeleton
    {
        public string? MinimumLevel { get; set; }
        private Logger? _serilogLogger;

        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                var message = RenderLoggingEvent(loggingEvent);
                var level = GetLogEventLevel(loggingEvent.Level.ToString());
                var exception = loggingEvent.GetException();
                _serilogLogger?.Write(level, exception, message);
                AfterWrite(level, exception, message);
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Error occurred while logging the event.", ex);
            }
        }

        protected override bool RequiresLayout => true;

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
#if !NET452
                    .Enrich.WithExceptionDetails()
#endif
                ;
            loggerConfig = Enrich(loggerConfig);
            loggerConfig = WriteTo(loggerConfig);
            _serilogLogger = loggerConfig.CreateLogger();
            AfterActivateOptions(_serilogLogger);
        }

        public override void OnClose()
        {
            base.OnClose();
            _serilogLogger?.Dispose();
        }

        protected virtual void AfterWrite(LogEventLevel level, Exception? exception, string messageTemplate)
        {
        }

        protected virtual void AfterActivateOptions(Logger logger)
        {
        }

        protected abstract LoggerConfiguration Enrich(LoggerConfiguration configuration);
        protected abstract LoggerConfiguration WriteTo(LoggerConfiguration configuration);
        protected abstract bool ValidateConfiguration();
    }
}