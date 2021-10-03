using StarWars.Core.Settings;
using Microsoft.Extensions.Localization;
using Serilog;

namespace StarWars.Core
{
    public static class StaticData
    {
        public static IStringLocalizer Localizer { get; set; }
        public static ILogger Logger { get; set; }
        public static PlatformSettings Settings { get; set; }
    }
}