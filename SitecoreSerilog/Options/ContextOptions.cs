namespace SitecoreSerilog.Options
{
    public class ContextOptions
    {
        public IDictionary<string, Func<object?>> Properties { get; } = new Dictionary<string, Func<object?>>();
        public string PropertyName { get; set; }
        public Func<bool>? Condition { get; }

        public ContextOptions(string propertyName, Func<bool>? condition = null)
        {
            PropertyName = propertyName;
            Condition = condition;
        }

        public ContextOptions WithProperty(string name, Func<object?> func)
        {
            Properties[name] = func;
            return this;
        }

        public static ContextOptions DefaultSitecoreContextOptions => new ContextOptions(Constants.OptionNames.Context)
            .WithProperty("Site", () => Sitecore.Context.Site?.Name)
            .WithProperty("Language", () => Sitecore.Context.Language?.Name)
            .WithProperty("Database", () => Sitecore.Context.Database?.Name);

        public static ContextOptions DefaultSitecoreUserOptions => new ContextOptions(Constants.OptionNames.User)
            .WithProperty("IsAuthenticated", () => Sitecore.Context.User?.IsAuthenticated)
            .WithProperty("Username", () => Sitecore.Context.User?.Name);

        public static ContextOptions DefaultJobOptions => new ContextOptions(Constants.OptionNames.Job, () => Sitecore.Context.Job != null)
            .WithProperty("Name", () => Sitecore.Context.Job?.Name)
            .WithProperty("State", () => Sitecore.Context.Job.Status?.State.ToString());
    }
}
