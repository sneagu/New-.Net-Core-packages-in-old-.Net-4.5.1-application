using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Config.ConfigFileConfigProvider
{
    public static class ConfigFileConfigurationExtensions
    {
        /// <summary>
        /// Adds configuration values of a *.config file to the ConfigurationBuilder
        /// </summary>
        /// <param name="builder">Builder to add configuration values to</param>
        /// <param name="configContents">Contents of *.config file</param>
        /// <param name="parsers">Additional parsers to use to parse the config contents</param>
        public static IConfigurationBuilder AddConfiguration(this IConfigurationBuilder builder, string configContents, params IConfigurationParser[] parsers)
        {
            if (configContents == null)
            {
                throw new ArgumentNullException("configContents");
            }
            else if (string.IsNullOrEmpty(configContents))
            {
                throw new ArgumentException("Contents for configuration cannot be empty.", "configContents");
            }

            return builder.Add(new ConfigFileConfigurationSource(configContents, false, false, parsers));
        }

        /// <summary>
        /// Adds configuration values for a *.config file to the ConfigurationBuilder
        /// </summary>
        /// <param name="builder">Builder to add configuration values to</param>
        /// <param name="path">Path to *.config file</param>
        public static IConfigurationBuilder AddConfigFile(this IConfigurationBuilder builder, string path)
        {
            return builder.AddConfigFile(path, optional: false);
        }

        /// <summary>
        /// Adds configuration values for a *.config file to the ConfigurationBuilder
        /// </summary>
        /// <param name="builder">Builder to add configuration values to</param>
        /// <param name="path">Path to *.config file</param>
        /// <param name="optional">true if file is optional; false otherwise</param>
        /// <param name="parsers">Additional parsers to use to parse the config file</param>
        public static IConfigurationBuilder AddConfigFile(this IConfigurationBuilder builder, string path, bool optional, params IConfigurationParser[] parsers)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            else if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path for configuration cannot be null/empty.", "path");
            }

            //var directory = System.IO.Path.GetDirectoryName(path);
            //var pathToFile = System.IO.Path.GetFileName(path);
            //while (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            //{
            //    pathToFile = System.IO.Path.Combine(System.IO.Path.GetFileName(directory), pathToFile);
            //    directory = System.IO.Path.GetDirectoryName(directory);
            //}
            if (!optional && !File.Exists(path))
            {
                throw new FileNotFoundException(string.Format("Could not find configuration file. File: [{0}]", path));
            }

            return builder.Add(new ConfigFileConfigurationSource(path, true, optional, parsers));
        }
    }
}
