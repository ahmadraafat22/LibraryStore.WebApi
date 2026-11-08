using LibraryStore.Application.Features.StoreSettings.Commands.UpdateStoreSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryStore.WebApi.Controllers.Admin
{
    [Route("api/admin/StoreSettings")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AdminStoreSettingsController : ControllerBase
    {

        private readonly IMediator _mediator;
        public AdminStoreSettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPut]
        public async Task<IActionResult> UpdateStoreSettings([FromBody] UpdateStoreSettingsCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
