using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private readonly ApplicationDbContext _context;

        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);

                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> UpdateUserPasswordAsync(Guid userId, Password password)
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new InvalidOperationException($"user with id: {userId} was null");
                }

                user.ChangePassword(password);

                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> UpdateUserNameAsync(Guid userId, string userName)
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new InvalidOperationException($"user with id: {userId} was null");
                }

                user.UpdateUserName(userName);

                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> UpdateUserEmailAsync(Guid userId, Email email)
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new InvalidOperationException($"user with id: {userId} was null");
                }

                user.UpdateUserEmail(email);

                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> ActivateUserAsync(Guid userId)
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new InvalidOperationException($"user with id: {userId} was null");
                }

                if (user.IsActive)
                {
                    user.DeactivateUser();
                }
                else
                {
                    user.ActivateUser();
                }

                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> DeleteUserAsync(Guid userId)
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new InvalidOperationException($"user with id: {userId} was null");
                }

                _context.Users.Remove(user);

                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<User>> GetUsersAsync()
        {
            try
            {
                return await _context.Users
                .AsNoTracking()
                .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            try
            {
                User? user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new InvalidOperationException($"user with id: {userId} was null");
                }

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> GetUserByEmailAndPasswordAsync(Email email, Password password)
        {
            try
            {
                User? user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserEmail.Value == email.Value && u.Password.Hash == password.Hash);

                if (user == null)
                {
                    throw new InvalidOperationException($"user with email: {email.Value} was null");
                }

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
