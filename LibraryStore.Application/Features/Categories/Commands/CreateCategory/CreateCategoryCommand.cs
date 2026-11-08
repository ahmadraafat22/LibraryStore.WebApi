using MediatR;

namespace LibraryStore.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
