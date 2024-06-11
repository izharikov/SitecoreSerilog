using Serilog;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Sitecore.Configuration;
using SitecoreSerilog.Appenders;
using SitecoreSerilog.Extensions;

namespace SitecoreSerilog.Example
{
    public class BaseSeqSitecoreAppender : BaseSitecoreSerilogAppender
    {
        public string ApiKey { get; set; }
        public string SeqHost { get; set; }

        // configure Enrichers here (use listed below or define your own enrichers):
        protected override LoggerConfiguration Enrich(LoggerConfiguration configuration)
        {
            return configuration
                    // application name enricher (so you know, which site is used)
                    .Enrich.WithApplicationName(() => Sitecore.Context.Site?.Name ?? "FallbackValue")
                    // spoke name enricher (so you know, which environment is used)
                    .Enrich.WithSpokeName(Settings.GetSetting("SpokeName"))
                    .Enrich.FromLogContext()
                    .Enrich.WithUtcTimestamp()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentUserName()
                    .Enrich.WithProcessId()
                    .Enrich.WithProcessName()
                    .Enrich.WithThreadId()
                    .Enrich.WithMemoryUsage()
                    // custom SitecoreContextEnricher - so you can see the Sitecore Context Details in log event
                    .Enrich.WithSitecoreContext(LogEventLevel.Error)
                    // custom HttpContextEnricher - so you can see HttpContext details in log event
                    .Enrich.WithHttpContext(LogEventLevel.Error)
                ;
        }

        // configure where Serilog should write
        protected override LoggerConfiguration WriteTo(LoggerConfiguration configuration)
        {
            return configuration
                // in this example I write to SEQ, but you can use any Sink you need
                .WriteTo.Seq(SeqHost, apiKey: ApiKey);
        }

        // validate your configuration (if returned false - Serilog won't write anything)
        protected override bool ValidateConfiguration()
        {
            return !string.IsNullOrEmpty(SeqHost);
        }

        protected override void AfterActivateOptions(Logger logger)
        {
            base.AfterActivateOptions(logger);
            SelfLog.Enable(ErrorHandler.Error);
        }
    }
}