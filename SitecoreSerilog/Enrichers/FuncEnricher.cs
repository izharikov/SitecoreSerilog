using System;
using Serilog.Core;
using Serilog.Events;

namespace SitecoreSerilog.Enrichers
{
    public class FuncEnricher : ILogEventEnricher
    {
        private readonly string _name;
        private readonly Func<LogEvent, object> _func;
        
        public FuncEnricher(string name, Func<object> func)
        {
            _name = name;
            _func = (_) => func();
        }
        
        public FuncEnricher(string name, Func<LogEvent, object> func)
        {
            _name = name;
            _func = func;
        }
        
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(_name, _func(logEvent)));
        }
    }
}