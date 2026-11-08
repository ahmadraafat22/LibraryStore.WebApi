using LibraryStore.Application.Features.Categories.Commands.CreateCategory;
using LibraryStore.Application.Features.Categories.Commands.SoftDeleteCategory;
using LibraryStore.Application.Features.Categories.Commands.UpdateCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryStore.WebApi.Controllers.Admin
{
    [Route("api/admin/Categories")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AdminCategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminCategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryCommand command)
        {
            var Id = await _mediator.Send(command);

            return Ok(Id);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            await _mediator.Send(new SoftDeleteCategoryCommand(id));
            return NoContent();
        }
    }
}
