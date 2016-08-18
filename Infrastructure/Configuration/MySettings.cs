using System;

namespace Infrastructure.Configuration
{
    public class MySettings
    {
        public string StringSetting { get; set; }

        public DateTime DateSetting { get; set; }

        public bool BooleanSetting { get; set; }

        public NestedSettings ObjectSettings { get; set; }
    }
}
