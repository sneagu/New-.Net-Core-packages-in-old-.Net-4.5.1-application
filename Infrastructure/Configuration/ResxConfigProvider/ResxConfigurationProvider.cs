using Microsoft.Extensions.Configuration;
using System;
using System.Xml.Linq;
using System.Linq;
using System.IO;

namespace Infrastructure.Configuration.ResxConfigProvider
{
    class ResxConfigurationProvider : FileConfigurationProvider
    {
        public ResxConfigurationProvider(FileConfigurationSource source) : base(source)
        {
        }

        public override void Load()
        {
            var file = Source.FileProvider.GetFileInfo(Source.Path);
            Data = XDocument
                .Load(file.PhysicalPath)
                .Descendants()
                .Where(x => x.Name == "data")
                .ToDictionary(x => x.Attribute("name").Value, x => x.Value);
        }

        public override void Load(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
