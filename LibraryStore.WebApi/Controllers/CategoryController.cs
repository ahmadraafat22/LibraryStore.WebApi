using LibraryStore.Application.Features.Categories.Commands.CreateCategory;
using LibraryStore.Application.Features.Categories.Commands.SoftDeleteCategory;
using LibraryStore.Application.Features.Categories.Commands.UpdateCategory;
using LibraryStore.Application.Features.Categories.Queries.GetAllCategories;
using LibraryStore.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LibraryStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryCommand command)
        {
            var Id = await _mediator.Send(command);

            return Ok(Id);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories([FromQuery] GetAllCategoriesQuery query)
        {
            return Ok(await _mediator.Send(query));
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            return Ok(await _mediator.Send(new GetCategoryByIdQuery(id)));
        }
        [Authorize(Roles = "Admin")]
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
