using Identity.Application.Interfaces;
using Identity.Domain.Entities;
using Identity.Domain.Exceptions;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(ApplicationDbContext context, ILoggerService logger)
        {
            _context = context;
            _logger = logger;
        }

        private readonly ApplicationDbContext _context;
        private readonly ILoggerService _logger;

        public async Task<User> CreateUserAsync(User user)
        {
            string methodName = $"{nameof(UserRepository)}.{nameof(CreateUserAsync)}";
            try
            {
                await _logger.LogInfo($"Creating user with email: {user.UserEmail.Value}", methodName);

                User? dbUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserEmail.Value == user.UserEmail.Value);

                if (dbUser != null)
                {
                    await _logger.LogWarning($"User with email {user.UserEmail.Value} already exists", methodName);
                    throw new InvalidEmailException("User with current email is already exists");
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await _logger.LogInfo($"User created successfully with ID: {user.Id}", methodName);
                return user;
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Error creating user with email: {user.UserEmail.Value}", methodName, ex.Message);
                throw;
            }
        }

        public async Task<User> UpdateUserPasswordAsync(Guid userId, Password password)
        {
            string methodName = $"{nameof(UserRepository)}.{nameof(UpdateUserPasswordAsync)}";
            try
            {
                await _logger.LogInfo($"Updating password for user ID: {userId}", methodName);

                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    await _logger.LogWarning($"User with ID {userId} not found", methodName);
                    throw new NotFoundException($"user with id: {userId} was null");
                }

                user.ChangePassword(password);
                await _context.SaveChangesAsync();

                await _logger.LogInfo($"Password updated successfully for user ID: {userId}", methodName);
                return user;
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Error updating password for user ID: {userId}", methodName, ex.Message);
                throw;
            }
        }

        public async Task<User> UpdateUserNameAsync(Guid userId, string userName)
        {
            string methodName = $"{nameof(UserRepository)}.{nameof(UpdateUserNameAsync)}";
            try
            {
                await _logger.LogInfo($"Updating username for user ID: {userId}", methodName);

                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    await _logger.LogWarning($"User with ID {userId} not found", methodName);
                    throw new NotFoundException($"user with id: {userId} was null");
                }

                user.UpdateUserName(userName);
                await _context.SaveChangesAsync();

                await _logger.LogInfo($"Username updated successfully for user ID: {userId}", methodName);
                return user;
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Error updating username for user ID: {userId}", methodName, ex.Message);
                throw;
            }
        }

        public async Task<User> UpdateUserEmailAsync(Guid userId, Email email)
        {
            string methodName = $"{nameof(UserRepository)}.{nameof(UpdateUserEmailAsync)}";
            try
            {
                await _logger.LogInfo($"Updating email for user ID: {userId} to {email.Value}", methodName);

                User? dbUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserEmail.Value == email.Value);

                if (dbUser != null)
                {
                    await _logger.LogWarning($"Email {email.Value} already exists in system", methodName);
                    throw new InvalidEmailException("User with current email is already exists");
                }

                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    await _logger.LogWarning($"User with ID {userId} not found", methodName);
                    throw new NotFoundException($"user with id: {userId} was null");
                }

                user.UpdateUserEmail(email);
                await _context.SaveChangesAsync();

                await _logger.LogInfo($"Email updated successfully for user ID: {userId}", methodName);
                return user;
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Error updating email for user ID: {userId}", methodName, ex.Message);
                throw;
            }
        }

        public async Task<User> ActivateUserAsync(Guid userId)
        {
            string methodName = $"{nameof(UserRepository)}.{nameof(ActivateUserAsync)}";
            try
            {
                await _logger.LogInfo($"Toggling activation status for user ID: {userId}", methodName);

                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    await _logger.LogWarning($"User with ID {userId} not found", methodName);
                    throw new NotFoundException($"user with id: {userId} was null");
                }

                if (user.IsActive)
                {
                    user.DeactivateUser();
                    await _logger.LogInfo($"User deactivated for user ID: {userId}", methodName);
                }
                else
                {
                    user.ActivateUser();
                    await _logger.LogInfo($"User activated for user ID: {userId}", methodName);
                }

                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Error toggling activation status for user ID: {userId}", methodName, ex.Message);
                throw;
            }
        }

        public async Task<User> DeleteUserAsync(Guid userId)
        {
            string methodName = $"{nameof(UserRepository)}.{nameof(DeleteUserAsync)}";
            try
            {
                await _logger.LogInfo($"Deleting user with ID: {userId}", methodName);

                User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    await _logger.LogWarning($"User with ID {userId} not found for deletion", methodName);
                    throw new NotFoundException($"user with id: {userId} was null");
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                await _logger.LogInfo($"User deleted successfully with ID: {userId}", methodName);
                return user;
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Error deleting user with ID: {userId}", methodName, ex.Message);
                throw;
            }
        }

        public async Task<List<User>> GetUsersAsync()
        {
            string methodName = $"{nameof(UserRepository)}.{nameof(GetUsersAsync)}";
            try
            {
                await _logger.LogInfo("Getting all users", methodName);

                var users = await _context.Users
                    .AsNoTracking()
                    .ToListAsync();

                await _logger.LogInfo($"Retrieved {users.Count} users successfully", methodName);
                return users;
            }
            catch (Exception ex)
            {
                await _logger.LogError("Error getting all users", methodName, ex.Message);
                throw;
            }
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            string methodName = $"{nameof(UserRepository)}.{nameof(GetUserByIdAsync)}";
            try
            {
                await _logger.LogInfo($"Getting user by ID: {userId}", methodName);

                User? user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    await _logger.LogWarning($"User with ID {userId} not found", methodName);
                    throw new NotFoundException($"user with id: {userId} was null");
                }

                await _logger.LogInfo($"User retrieved successfully with ID: {userId}", methodName);
                return user;
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Error getting user by ID: {userId}", methodName, ex.Message);
                throw;
            }
        }

        public async Task<User> GetUserByEmailAndPasswordAsync(Email email, Password password)
        {
            string methodName = $"{nameof(UserRepository)}.{nameof(GetUserByEmailAndPasswordAsync)}";
            try
            {
                await _logger.LogInfo($"Getting user by email: {email.Value}", methodName);

                User? user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserEmail.Value == email.Value && u.Password.Hash == password.Hash);

                if (user == null)
                {
                    await _logger.LogWarning($"User with email {email.Value} not found or password mismatch", methodName);
                    throw new NotFoundException($"user with email: {email.Value} was null");
                }

                await _logger.LogInfo($"User retrieved successfully with email: {email.Value}", methodName);
                return user;
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Error getting user by email: {email.Value}", methodName, ex.Message);
                throw;
            }
        }
    }
}
