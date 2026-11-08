using LibraryStore.Application.Features.Products.Queries.GetAllProducts;
using LibraryStore.Application.Features.Products.Queries.GetNew_Arrivals;
using LibraryStore.Application.Features.Products.Queries.GetProductById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private IMediator _mediator;
        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] GetAllProductsQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
        [HttpGet("new-arrivals")]
        public async Task<IActionResult> GetNewArrivals()
        {
            return Ok(await _mediator.Send(new GetNewArrivalsQuery()));
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            return Ok(await _mediator.Send(new GetProductByIdQuery(id)));
        }

    }
}
