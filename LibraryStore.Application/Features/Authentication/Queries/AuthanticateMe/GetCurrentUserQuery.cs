using MediatR;

namespace LibraryStore.Application.Features.Authentication.Queries.AuthanticateMe
{
    public class GetCurrentUserQuery : IRequest<GetCurrentUserDto>
    {
    }
}
