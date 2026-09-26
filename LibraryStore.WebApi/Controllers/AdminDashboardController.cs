using LibraryStore.Application.Features.Admin.Queries.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryStore.WebApi.Controllers
{
    [Route("api/[controller]")]
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
