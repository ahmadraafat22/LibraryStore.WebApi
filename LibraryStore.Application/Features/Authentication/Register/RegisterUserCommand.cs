using MediatR;

namespace LibraryStore.Application.Features.Authentication.Register
{
    public class RegisterUserCommand : IRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
