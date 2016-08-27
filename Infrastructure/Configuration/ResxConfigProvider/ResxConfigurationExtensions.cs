using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;

namespace Infrastructure.Configuration.ResxConfigProvider
{
    public static class ResxConfigurationExtensions
    {
        public static IConfigurationBuilder AddResxFile(this IConfigurationBuilder builder, string path)
        {
            return AddResxFile(builder, provider: null, path: path, optional: false, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddResxFile(this IConfigurationBuilder builder, string path, bool optional)
        {
            return AddResxFile(builder, provider: null, path: path, optional: optional, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddResxFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            return AddResxFile(builder, provider: null, path: path, optional: optional, reloadOnChange: reloadOnChange);
        }

        public static IConfigurationBuilder AddResxFile(this IConfigurationBuilder builder, IFileProvider provider,
            string path, bool optional, bool reloadOnChange)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("path");
            }

            var source = new ResxConfigurationSource
            {
                FileProvider = provider,
                Path = path,
                Optional = optional,
                ReloadOnChange = reloadOnChange
            };
            //source.ResolveFileProvider();
            builder.Add(source);
            return builder;
        }
    }
}
