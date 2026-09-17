using LibraryStore.Application.Features.Authentication.Login;
using LibraryStore.Application.Features.Authentication.Queries.AuthanticateMe;
using LibraryStore.Application.Features.Authentication.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryStore.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUserCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
        [HttpGet("me")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AuthanticateMe()
        {
            return Ok(await _mediator.Send(new GetCurrentUserQuery()));
        }
    }
}
