using System;
using Serilog;
using Serilog.Events;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using SitecoreSerilog.Appenders;
using SitecoreSerilog.Extensions;

namespace SitecoreSerilog.Example
{
    public abstract class BaseSitecoreMvcSerilogAppender : BaseSitecoreSerilogAppender
    {
        protected override LoggerConfiguration Enrich(LoggerConfiguration configuration)
        {
            return configuration
                    .Enrich.WithApplicationName(() => Sitecore.Context.Site?.Name ?? "FallbackValue")
                    .Enrich.WithSpokeName(Settings.GetSetting("SpokeName"))
                    .Enrich.FromLogContext()
                    .Enrich.WithUtcTimestamp()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentUserName()
                    .Enrich.WithProcessId()
                    .Enrich.WithProcessName()
                    .Enrich.WithThreadId()
                    .Enrich.WithMemoryUsage()
                    .Enrich.WithSitecoreContext(
                        minLevel: LogEventLevel.Error,
                        additionalItems: new (string, Func<Item>)[]
                        {
                            ("PageItem", () => PageContext.CurrentOrNull?.Item),
                            ("Datasource", () => RenderingContext.CurrentOrNull?.Rendering?.Item),
                        },
                        additionalContextObjects: new (string name, Func<object> contextObjectFunc)[]
                        {
                            ("RenderingId", () => RenderingContext.CurrentOrNull?.Rendering?.Id.ToString()),
                        }
                    )
                    .Enrich.WithHttpContext(LogEventLevel.Error)
                ;
        }
    }
}