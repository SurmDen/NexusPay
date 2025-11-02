using Identity.Application.User.DTOs;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Queries
{
    public class GetUserByEmailAndPasswordQueryHandler : IRequestHandler<GetUserByEmailAndPasswordQuery, UserDto>
    {
        public GetUserByEmailAndPasswordQueryHandler(IUserRepository userRepository, IMediator mediator)
        {
            _mediator = mediator;
            _userRepository = userRepository;
        }

        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public async Task<UserDto> Handle(GetUserByEmailAndPasswordQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Email email = new Email(request.Email);
                Password password = new Password(request.Password);

                var user = await _userRepository.GetUserByEmailAndPasswordAsync(email, password);

                var userDto = new UserDto()
                {
                    UserName = user.UserName,
                    Email = user.UserEmail.Value,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    Id = user.Id,
                };

                return userDto;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
