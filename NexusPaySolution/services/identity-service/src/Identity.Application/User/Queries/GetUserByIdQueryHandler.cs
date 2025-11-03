using Identity.Application.Interfaces;
using Identity.Application.User.DTOs;
using Identity.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Queries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        public GetUserByIdQueryHandler(IUserRepository userRepository, IMediator mediator, ILoggerService loggerService)
        {
            _loggerService = loggerService;
            _mediator = mediator;
            _userRepository = userRepository;
        }

        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;
        private readonly ILoggerService _loggerService;

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(request.Id);

                var userDto = new UserDto()
                {
                    Email = user.UserEmail.Value,
                    UserName = user.UserName,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    Id = user.Id,
                    RoleName = user.RoleName.Value
                };

                return userDto;
            }
            catch (Exception e)
            {
                await _loggerService.LogError(e.Message, "GetUserByIdQueryHandler.Handle", e.GetType().FullName);

                throw;
            }
        }
    }
}
