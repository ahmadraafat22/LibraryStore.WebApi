using LibraryStore.Application.Features.StoreSettings.Queries.GetStoreSettings;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreSettingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public StoreSettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetStoreSettings(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStoreSettingsQuery(), cancellationToken);
            return Ok(result);
        }
    }
}
