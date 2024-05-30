using System;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;
using Sitecore.Data;
using Sitecore.Data.Items;
using SitecoreSerilog.Options;

namespace SitecoreSerilog.Enrichers
{
    public class SitecoreContextEnricher : ILogEventEnricher
    {
        private readonly SitecoreContextEnricherOptions _options;

        public SitecoreContextEnricher(SitecoreContextEnricherOptions options)
        {
            _options = options;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Level < _options.MinLevel)
            {
                return;
            }


            var sitecore = new Dictionary<string, object>();

            var itemUris = new HashSet<ItemUri>();
            foreach (var kv in _options.Items)
            {
                var item = kv.Value();
                if (item != null && (!_options.DistinctItems || itemUris.Add(item.Uri)))
                {
                    AddSitecoreItemEnrich(kv.Value(), kv.Key);
                }
            }

            foreach (var contextOptions in _options.ContextOptions)
            {
                if (contextOptions.Condition != null && !contextOptions.Condition())
                {
                    continue;
                }
                var result = new Dictionary<string, object?>();
                foreach (var property in contextOptions.Properties)
                {
                    result[property.Key] = property.Value();
                }

                sitecore[contextOptions.PropertyName] = result;
            }

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(_options.PropertyName, sitecore, true));
            return;

            void AddSitecoreItemEnrich(Item? item, string name)
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