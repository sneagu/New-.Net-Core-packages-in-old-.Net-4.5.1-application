using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;

namespace Infrastructure.Configuration.ResxConfigProvider
{
    public static class ResxConfigurationExtensions
    {
        /// <summary>
        /// Adds the Resx configuration provider at <paramref name="path"/> to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="path">Path relative to the base path stored in 
        /// <see cref="IConfigurationBuilder.Properties"/> of <paramref name="builder"/>.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddResxFile(this IConfigurationBuilder builder, string path)
        {
            return AddResxFile(builder, provider: null, path: path, optional: false, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddResxFile(this IConfigurationBuilder builder, string path, bool optional)
        {
            return AddResxFile(builder, provider: null, path: path, optional: optional, reloadOnChange: false);
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
