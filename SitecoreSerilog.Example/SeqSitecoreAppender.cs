using Serilog;

namespace SitecoreSerilog.Example
{
    public class SeqSitecoreAppender : BaseSitecoreMvcSerilogAppender
    {
        public string ApiKey { get; set; }
        public string SeqHost { get; set; }

        protected override LoggerConfiguration WriteTo(LoggerConfiguration configuration)
        {
            return configuration
                .WriteTo.Seq(SeqHost, apiKey: ApiKey);
        }

        protected override bool ValidateConfiguration()
        {
            return !string.IsNullOrEmpty(SeqHost);
        }
    }
}