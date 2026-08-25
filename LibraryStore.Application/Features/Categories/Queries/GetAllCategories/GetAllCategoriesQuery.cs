using MediatR;

namespace LibraryStore.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQuery : IRequest<ICollection<GetAllCategoryQueryDto>>
    {
    }
}
