using Application.Common.Wrappers;
using Application.DTOs.Authentication.Request;
using Application.Interfaces.Repositories;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Authentication.Commands;

public class RegisterCommand : IRequest<Response<int>>, IRegister
{
    public RegisterRequestDto dto { get; set; }

    public void Register(TypeAdapterConfig config)
    {
        config.ForType<RegisterRequestDto, Domain.Entities.Auth>();
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Response<int>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<int>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = request.dto.Adapt<Domain.Entities.Auth>();

                await _unitOfWork.Auth.AddAsync(entity);

                await _unitOfWork.CommitAsync();

                return new Response<int>(entity.Id, new CrudResponse(Constants.CrudOperation.Create));
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
