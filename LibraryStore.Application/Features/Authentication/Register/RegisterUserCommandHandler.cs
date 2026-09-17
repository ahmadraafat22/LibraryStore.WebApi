using LibraryStore.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LibraryStore.Application.Features.Authentication.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public RegisterUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // mapping user data 
            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,

            };
            // trying to createing the user 
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }
            // if creation succeced add to role 
            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(",", roleResult.Errors.Select(e => e.Description));
                throw new Exception(errors);
            }
        }
    }
}
