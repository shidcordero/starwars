using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace StarWars.API.Controllers
{
    public class ErrorController : BaseController
    {
        public ErrorController(
            IMemoryCache cache,
            ILogger<ErrorController> logger,
            IStringLocalizer<BaseController> localizer)
            : base(cache, logger, localizer)
        {
        }

        [HttpGet("404")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public ActionResult Get404()
        {
            return NotFound();
        }

        [HttpGet("401")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public ActionResult Get401()
        {
            return Unauthorized();
        }

        [HttpGet("400")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public ActionResult Get400()
        {
            return BadRequest();
        }
    }
}