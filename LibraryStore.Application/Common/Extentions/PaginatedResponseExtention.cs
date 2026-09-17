using LibraryStore.Application.Common.Responses;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore.Application.Common.Extentions
{
    public static class PaginatedResponseExtention
    {
        public static async Task<PaginatedResponse<T>> ToPaginatedResponse<T>(this IQueryable<T> query
            , int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            if (pageNumber <= 0)
                pageNumber = 1;
            if (pageSize <= 0)
                pageSize = 3;
            if (pageSize > 50)
                pageSize = 50;

            var totalCount = await query.CountAsync(cancellationToken);

            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedResponse<T>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((totalCount) / (double)pageSize)
            };
        }
    }
}
