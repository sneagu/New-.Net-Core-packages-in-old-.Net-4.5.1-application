using System.Data.Entity;

namespace Infrastructure.Logging.DbLoggerProvider
{
    public class LoggingDbContext : DbContext
    {
        public LoggingDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public DbSet<Log> Logs { get; set; }
    }
}
