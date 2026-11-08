using LibraryStore.Application.Common.Responses;
using MediatR;

namespace LibraryStore.Application.Features.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesQuery : IRequest<PaginatedResponse<GetAllCategoryQueryDto>>
    {
        public string? Search { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 3;
    }
}
