using Application.Common.Exceptions;
using Application.Common.Wrappers;
using Application.DTOs.Authentication.Request;
using Application.DTOs.Authentication.Response;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using MediatR;

namespace Application.Features.Authentication.Commands;

public class LoginCommand : IRequest<Response<UserToken>>
{
    public LoginRequestDto dto { get; set; }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Response<UserToken>>
    {
        private readonly IAuthenticateService _authenticateService;
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(IAuthenticateService authenticateService, IUnitOfWork unitOfWork)
        {
            _authenticateService = authenticateService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<UserToken>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var isAccountExists = await _unitOfWork.Auth.GetAsync<AuthResponseDto>(a => string.Equals(a.EmailAddress, request.dto.EmailAddress.Trim())
                                                                                     && string.Equals(a.Password, request.dto.Password.Trim()));

            if (isAccountExists is not null)
            {
                return new Response<UserToken>(_authenticateService.BuildToken(request.dto.EmailAddress));
            }

            throw new ResponseException("The registered account does not exist");
        }
    }
}