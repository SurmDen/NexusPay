using Identity.Application.User.DTOs;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Queries
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
    {
        public GetUsersQueryHandler(IUserRepository userRepository, IMediator mediator)
        {
            _mediator = mediator;
            _userRepository = userRepository;
        }

        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userRepository.GetUsersAsync();

                var userDtos = users.Select(u =>
                {
                    return new UserDto()
                    {
                        UserName = u.UserName,
                        Email = u.UserEmail.Value,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt,
                        Id = u.Id,
                        RoleName = u.RoleName.Value
                    };
                });

                return userDtos.ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
