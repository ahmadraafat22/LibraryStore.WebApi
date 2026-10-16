using LibraryStore.Application.Features.Orders.Queries.GetAllOrders;
using LibraryStore.Application.Features.Orders.Queries.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryStore.WebApi.Controllers.Admin
{
    [Route("api/admin/Orders")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AdminOrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminOrderController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromQuery] GetAllOrdersQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            return Ok(await _mediator.Send(new GetOrderByIdQuery(id)));
        }
    }
}
