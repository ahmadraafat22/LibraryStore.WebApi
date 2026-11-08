using LibraryStore.Domain.Common.Enums;
using MediatR;

namespace LibraryStore.Application.Features.Admin.Queries.Dashboard
{
    public class GetDashboardStatsQuery : IRequest<DashboardStatsDto>
    {
        public DashboardPeriod? DashboardPeriod { get; set; }
    }
}
