using Application.Common.Wrappers;
using Application.DTOs.Authentication.Request;
using Application.DTOs.Authentication.Response;
using Application.Interfaces.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Authentication
{
    public class LoginCommand : IRequest<Response<UserToken>>
    {
        public LoginRequestDto dto { get; set; }

        public class LoginCommandHandler : IRequestHandler<LoginCommand, Response<UserToken>>
        {
            private readonly IAuthenticateService _authenticateService;

            public LoginCommandHandler(IAuthenticateService authenticateService)
            {
                _authenticateService = authenticateService;
            }

            public async Task<Response<UserToken>> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                return new Response<UserToken>(_authenticateService.BuildToken(request.dto.Username));
            }
        }
    }
}