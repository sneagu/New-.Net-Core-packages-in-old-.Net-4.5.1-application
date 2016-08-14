using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Configuration.ConfigDbProvider
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
