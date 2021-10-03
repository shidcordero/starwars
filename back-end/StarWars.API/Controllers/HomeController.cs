using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace StarWars.API.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(
            IMemoryCache cache,
            ILogger<HomeController> logger,
            IStringLocalizer<BaseController> localizer)
            : base(cache, logger, localizer)
        {
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public string Get()
        {
            return "Alive";
        }
    }
}