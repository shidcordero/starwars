using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace StarWars.API.Controllers
{
    public class FlushController : BaseController
    {
        public FlushController(
            IMemoryCache cache,
            ILogger<FlushController> logger,
            IStringLocalizer<BaseController> localizer)
            : base(cache, logger, localizer)
        {
        }

        [HttpGet]
        public void Clear()
        {
            var prop = Cache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);

            if (prop != null)
            {
                var innerCache = prop.GetValue(Cache);

                if (innerCache != null)
                {
                    var clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
                    if (clearMethod != null)
                    {
                        clearMethod.Invoke(innerCache, null);
                    }
                }
            }
        }
    }
}