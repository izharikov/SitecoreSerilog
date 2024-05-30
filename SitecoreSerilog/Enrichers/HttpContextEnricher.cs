using System.Web;
using Serilog.Core;
using Serilog.Events;

namespace SitecoreSerilog.Enrichers
{
    public class HttpContextEnricher : ILogEventEnricher
    {
        private readonly LogEventLevel _minLevel;
        
        public HttpContextEnricher(LogEventLevel minLevel)
        {
            _minLevel = minLevel;
        }
        
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Level < _minLevel)
            {
                return;
            }
            
            var request = HttpContext.Current?.Request;
            if (request != null)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("HttpRequest", new
                {
                    Url = request.Url.ToString(),
                    Method = request.HttpMethod,
                }, true));
            }
        }
    }
}