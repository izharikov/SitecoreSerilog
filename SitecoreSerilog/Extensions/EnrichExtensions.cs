using System;
using log4net.helpers;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Sitecore.Data.Items;
using SitecoreSerilog.Enrichers;

namespace SitecoreSerilog.Extensions
{
    public static class EnrichExtensions
    {
        public static LoggerConfiguration WithFuncEnricher(this LoggerEnrichmentConfiguration enrichmentConfiguration,
            string name, Func<object> func)
        {
            return enrichmentConfiguration != null
                ? enrichmentConfiguration.With(new FuncEnricher(name, func))
                : throw new ArgumentNullException(nameof(enrichmentConfiguration));
        }

        public static LoggerConfiguration WithFuncEnricher(this LoggerEnrichmentConfiguration enrichmentConfiguration,
            string name, Func<LogEvent, object> func)
        {
            return enrichmentConfiguration != null
                ? enrichmentConfiguration.With(new FuncEnricher(name, func))
                : throw new ArgumentNullException(nameof(enrichmentConfiguration));
        }

        public static LoggerConfiguration WithSitecoreContext(
            this LoggerEnrichmentConfiguration enrichmentConfiguration, LogEventLevel minLevel,
            (string name, Func<Item> itemFunc)[] additionalItems = null,
            (string name, Func<object> contextObjectFunc)[] additionalContextObjects = null)
        {
            return enrichmentConfiguration != null
                ? enrichmentConfiguration.With(new SitecoreContextEnricher(minLevel,
                    additionalItems ?? Array.Empty<(string, Func<Item>)>(),
                    additionalContextObjects ?? Array.Empty<(string, Func<object>)>()))
                : throw new ArgumentNullException(nameof(enrichmentConfiguration));
        }

        public static LoggerConfiguration WithHttpContext(this LoggerEnrichmentConfiguration enrichmentConfiguration,
            LogEventLevel minLevel)
        {
            return enrichmentConfiguration != null
                ? enrichmentConfiguration.With(new HttpContextEnricher(minLevel))
                : throw new ArgumentNullException(nameof(enrichmentConfiguration));
        }

        public static LoggerConfiguration WithUtcTimestamp(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            return enrichmentConfiguration.WithFuncEnricher("UtcTimestamp",
                (logEvent) => logEvent.Timestamp.UtcDateTime);
        }
        
        public static LoggerConfiguration WithThreadId(this LoggerEnrichmentConfiguration enrichmentConfiguration)
        {
            return enrichmentConfiguration.WithFuncEnricher("ThreadId",
                () => SystemInfo.CurrentThreadId);
        }

        public static LoggerConfiguration WithSpokeName(this LoggerEnrichmentConfiguration enrichmentConfiguration, string spokeName)
        {
            return enrichmentConfiguration.WithProperty("SpokeName", spokeName);
        }
        
        public static LoggerConfiguration WithApplicationName(this LoggerEnrichmentConfiguration enrichmentConfiguration, Func<string> applicationNameFunc)
        {
            return enrichmentConfiguration.WithFuncEnricher("ApplicationName", applicationNameFunc);
        }
    }
}