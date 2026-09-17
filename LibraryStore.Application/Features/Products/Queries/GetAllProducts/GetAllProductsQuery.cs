using LibraryStore.Application.Common.Responses;
using MediatR;

namespace LibraryStore.Application.Features.Products.Queries.GetAllProducts
{
    public class GetAllProductsQuery : IRequest<PaginatedResponse<GetAllProductsDto>>
    {
        public string? Search { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 3;
    }
}
