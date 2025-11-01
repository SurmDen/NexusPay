using Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Repositories
{
    public interface IUserRepository
    {
        public Task<Guid> CreateUserAsync(string name, string email, string password);

        public Task UpdateUserPasswordAsync(Guid userId, string password);

        public Task UpdateUserNameAsync(Guid userId, string userName);

        public Task UpdateUserEmailAsync(Guid userId, string email);

        public Task ActivateUserAsync(Guid userId);

        public Task DeleteUserAsync(Guid userId);

        public Task<List<User>> GetUsersAsync();

        public Task<User> GetUserByIdAsync(Guid userId);

        public Task<User> GetUserByEmailAndPasswordAsync(string email, string password);
    }
}
