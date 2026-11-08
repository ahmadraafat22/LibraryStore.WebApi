using LibraryStore.Application.Features.Admin.Queries.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryStore.WebApi.Controllers.Admin
{
    [Route("api/admin/Dashboard")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private IMediator _mediator;
        public AdminDashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats([FromQuery] GetDashboardStatsQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
    }
}
