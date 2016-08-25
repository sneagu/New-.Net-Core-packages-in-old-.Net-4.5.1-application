using System;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Configuration.DbConfigProvider
{
    public class EntityFrameworkConfigurationSource : IConfigurationSource
    {
        private string NameOrConnectionString { get; set; }

        public EntityFrameworkConfigurationSource(string nameOrConnectionString)
        {
            NameOrConnectionString = nameOrConnectionString;
        }
        
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new EntityFrameworkConfigurationProvider(NameOrConnectionString);
        }
    }
}