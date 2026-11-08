using MediatR;

namespace LibraryStore.Application.Features.Categories.Commands.SoftDeleteCategory
{
    public class SoftDeleteCategoryCommand : IRequest
    {
        public Guid Id { get; set; }
        public SoftDeleteCategoryCommand(Guid id)
        {
            Id = id;
        }
    }
}
