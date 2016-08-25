using System.Data.Entity;

namespace Infrastructure.Configuration.DbConfigProvider
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
