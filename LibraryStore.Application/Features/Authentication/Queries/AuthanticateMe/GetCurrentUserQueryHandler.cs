using LibraryStore.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LibraryStore.Application.Features.Authentication.Queries.AuthanticateMe
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, GetCurrentUserDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        public GetCurrentUserQueryHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _httpContext = httpContext;
        }

        public async Task<GetCurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var user = _httpContext.HttpContext.User;
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new Exception("user id not found ");
            }
            var applicationUser = await _userManager.FindByIdAsync(userId);

            if (applicationUser == null)
            {
                throw new Exception("user not found !");
            }

            var roles = await _userManager.GetRolesAsync(applicationUser);

            var UserDto = new GetCurrentUserDto
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                Roles = roles
            };

            return UserDto;
        }
    }
}
