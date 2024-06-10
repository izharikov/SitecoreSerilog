using Serilog;
using Serilog.Events;
using Sitecore.Configuration;
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
                    .Enrich.WithSitecoreContext(options =>
                    {
                        // configure min log level: 
                        options.MinLevel = LogEventLevel.Error;
                        // extend default enrich object:
                        options
                            // add current Sitecore MVC page context
                            .WithItem("PageItem", () => PageContext.CurrentOrNull?.Item)
                            // add current rendering datasource
                            .WithItem("Datasource", () => RenderingContext.CurrentOrNull?.Rendering?.Item)
                            .WithContextOption(Constants.OptionNames.Context,
                                context =>
                                {
                                    // add current RenderingId
                                    context.WithProperty("RenderingId",
                                        () => RenderingContext.CurrentOrNull?.Rendering?.Id.ToString());
                                })
                            ;
                    })
                    .Enrich.WithHttpContext(LogEventLevel.Error)
                ;
        }
    }
}