using System;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;
using Sitecore.Data.Items;

namespace SitecoreSerilog.Enrichers
{
    public class SitecoreContextEnricher : ILogEventEnricher
    {
        private readonly LogEventLevel _minLevel;
        private readonly (string name, Func<Item> itemFunc)[] _additionalItems;
        private readonly (string name, Func<object> contextObjectFunc)[] _additionalContextObjects;

        public SitecoreContextEnricher(LogEventLevel minLevel, (string name, Func<Item> itemFunc)[] additionalItems,
            (string name, Func<object> contextObjectFunc)[] additionalContextObjects)
        {
            _minLevel = minLevel;
            _additionalItems = additionalItems;
            _additionalContextObjects = additionalContextObjects;
        }
        
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Level < _minLevel)
            {
                return;
            }
            
            var contextItem = Sitecore.Context.Item;
            
            var sitecore = new Dictionary<string, object>();
            
            AddSitecoreItemEnrich(contextItem, "ContextItem");
            foreach (var (name, itemFunc) in _additionalItems)
            {
                var item = itemFunc();
                if (contextItem?.Uri != item?.Uri)
                {
                    AddSitecoreItemEnrich(item, name);
                }
            }
            
            dynamic scContext = new
            {
                Site = Sitecore.Context.Site?.Name,
                Language = Sitecore.Context.Language?.Name,
                Database = Sitecore.Context.Database?.Name,
            };

            foreach (var (name, contextObjectFunc) in _additionalContextObjects)
            {
                scContext[name] = contextObjectFunc();
            }

            sitecore["Context"] = scContext;
            
            sitecore["User"] = new
            {
                IsAuthenticated = Sitecore.Context.User?.IsAuthenticated,
                Username = Sitecore.Context.User?.Name,
            };
            
            var job = Sitecore.Context.Job;
            if (job != null)
            {
                sitecore["Job"] = new
                {
                    Name = job.Name,
                    State = job.Status?.State.ToString(),
                };
            }
            
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("SC", sitecore, true));
            return;
            
            void AddSitecoreItemEnrich(Item item, string name)
            {
                if (item == null)
                {
                    return;
                }
                
                sitecore[name] = new
                {
                    Id = item.ID.ToString(),
                    Uri = item.Uri?.ToString(),
                    Path = item.Paths.FullPath,
                    Language = item.Language?.Name,
                    Database = item.Database?.Name,
                };
            }
        }
    }
}