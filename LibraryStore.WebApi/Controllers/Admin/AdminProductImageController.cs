using LibraryStore.Application.Features.ProductImages.Commands.CreateImage;
using LibraryStore.Application.Features.ProductImages.Commands.SoftDeleteProductImage;
using LibraryStore.Application.Features.ProductImages.Commands.UpdateProductImage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryStore.WebApi.Controllers.Admin
{
    [Route("api/admin/ProductImage")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AdminProductImageController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminProductImageController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreateImage(CreateProductImageCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            await _mediator.Send(new SoftDeleteProductImageCommand(id));
            return NoContent();
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateImage(Guid id, UpdateProductImageCommand command)
        {
            command.ProductImageId = id;
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
