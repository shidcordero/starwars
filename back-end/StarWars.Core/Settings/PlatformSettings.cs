using System;

namespace StarWars.Core.Settings
{
    public class PlatformSettings
    {
        public string BaseUrl { get; set; }
        public string AllowedOrigins { get; set; }
        public string WebsiteBase { get; set; }
        public string SwapiUrl { get; set; }
        public int? MaxPageSize { get; set; }
        public int? DefaultPageSize { get; set; }
    }
}