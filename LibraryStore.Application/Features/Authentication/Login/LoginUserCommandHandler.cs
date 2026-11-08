using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LibraryStore.Application.Features.Authentication.Login
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        public LoginUserCommandHandler(IJwtService jwtService, UserManager<ApplicationUser> userManager)
        {
            _jwtService = jwtService;
            _userManager = userManager;
        }

        public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new Exception("Invalid Email or Password !");
            }

            var isPassswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPassswordValid)
            {
                throw new Exception("Invalid Email or Password !");
            }
            return await _jwtService.GenerateToken(user);

        }
    }
}
