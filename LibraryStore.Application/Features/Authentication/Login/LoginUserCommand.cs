using MediatR;

namespace LibraryStore.Application.Features.Authentication.Login
{
    public class LoginUserCommand : IRequest<string>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
