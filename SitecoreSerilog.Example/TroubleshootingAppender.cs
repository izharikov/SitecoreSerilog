using Serilog.Core;
using Serilog.Debugging;
using SitecoreSerilog.Appenders;

namespace SitecoreSerilog.Example
{
    public abstract class TroubleshootingAppender : BaseSitecoreSerilogAppender
    {
        // called after Serilog logger configured
        protected override void AfterActivateOptions(Logger logger)
        {
            base.AfterActivateOptions(logger);
            // configure Serilog internal output into default Sitecore output (need to be configured once)
            SelfLog.Enable(ErrorHandler.Error);
        }
    }
}