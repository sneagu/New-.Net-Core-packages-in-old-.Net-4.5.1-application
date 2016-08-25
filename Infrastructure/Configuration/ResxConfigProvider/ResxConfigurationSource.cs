using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Infrastructure.Configuration.ResxConfigProvider
{
    class ResxConfigurationSource : FileConfigurationSource
    {
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            FileProvider = FileProvider ?? builder.GetFileProvider();
            return new ResxConfigurationProvider(this);
        }
    }
}
