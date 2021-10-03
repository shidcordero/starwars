using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace StarWars.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected internal IStringLocalizer<BaseController> Localizer { get; private set; }
        protected internal IMemoryCache Cache { get; private set; }
        protected internal ILogger Logger { get; private set; }
        protected internal int CacheExpirationMinutes { get; private set; }

        protected string? PlatformUserId { get; private set; }

        public BaseController(IMemoryCache cache, ILogger logger, IStringLocalizer<BaseController> localizer)
        {
            this.Cache = cache;
            this.Logger = logger;
            this.Localizer = localizer;
            this.CacheExpirationMinutes = 1440;
        }
    }
}