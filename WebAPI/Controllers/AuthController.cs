using Application.DTOs.Authentication.Request;
using Application.Features.Authentication.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Authentication
{
    public class AuthController : BaseApiController
    {
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync(RegisterRequestDto dto)
        {
            return Ok(await Mediator.Send(new RegisterCommand() { dto = dto }));
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(LoginRequestDto dto)
        {
            return Ok(await Mediator.Send(new LoginCommand() { dto = dto }));
        }

    }
}