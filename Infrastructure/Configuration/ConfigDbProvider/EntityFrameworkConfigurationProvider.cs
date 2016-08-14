using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Configuration.ConfigDbProvider
{
    public class EntityFrameworkConfigurationProvider : ConfigurationProvider
    {
        public EntityFrameworkConfigurationProvider(string nameOrConnectionString)
        {
            NameOrConnectionString = nameOrConnectionString;
        }

        private string NameOrConnectionString { get; set; }

        public override void Load()
        {
            using (var dbContext = new ConfigurationDbContext(NameOrConnectionString))
            {
                //dbContext.Database.EnsureCreated();
                dbContext.Database.CreateIfNotExists();
                Data = !dbContext.ConfigurationValues.Any()
                    ? CreateAndSaveDefaultValues(dbContext)
                    : dbContext.ConfigurationValues.ToDictionary(c => c.Id, c => c.Value);
            }

        }

        private static IDictionary<string, string> CreateAndSaveDefaultValues(
            ConfigurationDbContext dbContext)
        {
            var configValues = new Dictionary<string, string>
                {
                    { "key1", "value_from_ef_1" },
                    { "key2", "value_from_ef_2" }
                };
            dbContext.ConfigurationValues.AddRange(configValues
                .Select(kvp => new ConfigurationValue { Id = kvp.Key, Value = kvp.Value })
                .ToArray());
            dbContext.SaveChanges();
            return configValues;
        }
    }
}