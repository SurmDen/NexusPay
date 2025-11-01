using Identity.Domain.Entities;
using Identity.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Repositories
{
    public interface IUserRepository
    {
        public Task<User> CreateUserAsync(User user);

        public Task<User> UpdateUserPasswordAsync(Guid userId, Password password);

        public Task<User> UpdateUserNameAsync(Guid userId, string userName);

        public Task<User> UpdateUserEmailAsync(Guid userId, Email email);

        public Task<User> ActivateUserAsync(Guid userId);

        public Task<User> DeleteUserAsync(Guid userId);

        public Task<List<User>> GetUsersAsync();

        public Task<User> GetUserByIdAsync(Guid userId);

        public Task<User> GetUserByEmailAndPasswordAsync(Email email, Password password);
    }
}
