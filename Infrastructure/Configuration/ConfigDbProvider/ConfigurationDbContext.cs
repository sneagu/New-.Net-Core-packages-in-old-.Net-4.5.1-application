using System.Data.Entity;

namespace Infrastructure.Configuration.ConfigDbProvider
{
    public class ConfigurationDbContext : DbContext
    {
        public ConfigurationDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public DbSet<ConfigurationValue> ConfigurationValues { get; set; }
    }
}
