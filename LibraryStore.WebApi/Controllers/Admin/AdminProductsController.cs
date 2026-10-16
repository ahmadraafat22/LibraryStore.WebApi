using LibraryStore.Application.Features.Products.Commands.CreateProduct;
using LibraryStore.Application.Features.Products.Commands.SoftDeleteProduct;
using LibraryStore.Application.Features.Products.Commands.UpdateProduct;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryStore.WebApi.Controllers.Admin
{
    [Route("api/admin/products")]
    [ApiController]
    public class AdminProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductCommand command)
        {
            return Ok(await _mediator.Send(command));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            await _mediator.Send(new SoftDeleteProductCommand(id));
            return NoContent();
        }
    }
}
