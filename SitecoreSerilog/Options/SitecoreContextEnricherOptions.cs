using Serilog.Events;
using Sitecore.Data.Items;

namespace SitecoreSerilog.Options
{
    public class SitecoreContextEnricherOptions
    {
        public LogEventLevel MinLevel { get; set; }
        public IDictionary<string, Func<Item?>> Items { get; } = new Dictionary<string, Func<Item?>>();
        public ICollection<ContextOptions> ContextOptions { get; } = new List<ContextOptions>();
        public string PropertyName { get; set; } = Constants.DefaultFields.ScContext;
        public bool DistinctItems { get; set; } = true;

        public SitecoreContextEnricherOptions WithItem(string item, Func<Item?> func)
        {
            Items[item] = func;
            return this;
        }

        public SitecoreContextEnricherOptions WithContextOption(ContextOptions option)
        {
            ContextOptions.Add(option);
            return this;
        }

        public SitecoreContextEnricherOptions WithContextOption(string name, Action<ContextOptions> extend)
        {
            var contextOptions = ContextOptions.FirstOrDefault(x => x.PropertyName == name);
            if (contextOptions != null)
            {
                extend(contextOptions);
            }

            return this;
        }
        
        public static SitecoreContextEnricherOptions Default => new SitecoreContextEnricherOptions()
            {
                MinLevel = LogEventLevel.Error,
            }
            .WithItem(Constants.DefaultFields.ContextItem, () => Sitecore.Context.Item)
            .WithContextOption(Options.ContextOptions.DefaultSitecoreContextOptions)
            .WithContextOption(Options.ContextOptions.DefaultSitecoreUserOptions)
            .WithContextOption(Options.ContextOptions.DefaultJobOptions)
        ;
    }
}