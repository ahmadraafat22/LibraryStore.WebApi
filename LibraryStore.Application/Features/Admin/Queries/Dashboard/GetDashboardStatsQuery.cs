using MediatR;

namespace LibraryStore.Application.Features.Admin.Queries.Dashboard
{
    public class GetDashboardStatsQuery : IRequest<DashboardStatsDto>
    {
    }
}
