using Microsoft.Extensions.Configuration;

namespace Infrastructure.Configuration.DbConfigProvider
{
    public static class EntityFrameworkExtensions
    {
        public static IConfigurationBuilder AddEntityFrameworkConfig(
            this IConfigurationBuilder builder, string nameOrConnectionString)
        {
            return builder.Add(new EntityFrameworkConfigurationSource(nameOrConnectionString));
        }
    }
}
