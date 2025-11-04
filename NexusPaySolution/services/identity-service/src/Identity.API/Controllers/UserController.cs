using Identity.Application.Interfaces;
using Identity.Application.User.Commands;
using Identity.Application.User.DTOs;
using Identity.Application.User.Queries;
using Identity.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    //port 5001
    [ApiController]
    [Route("api/v1/user")]
    public class UserController : ControllerBase
    {
        public UserController(IMediator mediator, ITokenService tokenService, ILoggerService logger)
        {
            _mediator = mediator;
            _tokenService = tokenService;
            _logger = logger;
        }

        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;
        private readonly ILoggerService _logger;

        
        [HttpPost("create")]
        public async Task<IActionResult> CreateUserAsync([FromBody] RegisterUserCommand registerUser)
        {
            string methodName = $"{nameof(UserController)}.{nameof(CreateUserAsync)}";

            try
            {
                await _logger.LogInfo("Starting user creation", methodName);

                if (!ModelState.IsValid)
                {
                    await _logger.LogWarning("Invalid model state for user creation", methodName);
                    return BadRequest(new { message = "invalid register parameters", code = 400 });
                }

                UserDto user = await _mediator.Send(registerUser);

                string token = _tokenService.GenerateToken(user.UserName, user.Email, user.Id, user.RoleName);

                await _logger.LogInfo($"User created successfully with ID: {user.Id}", methodName);
                return Ok(new { message = "user created, but not active. Confirmation code sent to email address", user = user, jwt_token = token, code = 200 });
            }
            catch (InvalidPasswordException e)
            {
                await _logger.LogWarning($"Invalid password exception: {e.Message}", methodName);
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (InvalidUserException e)
            {
                await _logger.LogWarning($"Invalid user exception: {e.Message}", methodName);
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (InvalidEmailException e)
            {
                await _logger.LogWarning($"Invalid email exception: {e.Message}", methodName);
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (NotFoundException e)
            {
                await _logger.LogWarning($"Not found exception: {e.Message}", methodName);
                return NotFound(new { message = e.Message });
            }
            catch (Exception ex)
            {
                await _logger.LogError("Unexpected error during user creation", methodName, ex.Message);
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        //[Authorize(User,Admin)]
        [HttpPost("email/confirm")]
        public async Task<IActionResult> ConfirmCodeAsync([FromBody] ConfirmEmailCommand emailCommand)
        {
            string methodName = $"{nameof(UserController)}.{nameof(ConfirmCodeAsync)}";

            try
            {
                await _logger.LogInfo("Starting email confirmation", methodName);

                if (!ModelState.IsValid)
                {
                    await _logger.LogWarning("Invalid model state for email confirmation", methodName);
                    return BadRequest(new { message = "invalid confirmation parameters", code = 400 });
                }

                bool codesEquals = await _mediator.Send(emailCommand);

                if (codesEquals)
                {
                    await _logger.LogInfo("Email confirmed successfully", methodName);
                    return Ok(new { message = "email confirmed, user is active", code = 200 });
                }
                else
                {
                    await _logger.LogWarning("Invalid confirmation code provided", methodName);
                    return BadRequest(new { message = "invalid code, try again", code = 400 });
                }
            }
            catch (ArgumentException e)
            {
                await _logger.LogWarning($"Argument exception: {e.Message}", methodName);
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (NotFoundException e)
            {
                await _logger.LogWarning($"Not found exception: {e.Message}", methodName);
                return NotFound(new { message = e.Message });
            }
            catch (Exception ex)
            {
                await _logger.LogError("Unexpected error during email confirmation", methodName, ex.Message);
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        //[Authorize(User,Admin)]
        [HttpPost("code/refresh")]
        public async Task<IActionResult> RefreshConfirmationCodeAsync([FromBody] GenerateCodeCommand generateCodeCommand)
        {
            string methodName = $"{nameof(UserController)}.{nameof(RefreshConfirmationCodeAsync)}";

            try
            {
                await _logger.LogInfo("Refreshing confirmation code", methodName);

                if (!ModelState.IsValid)
                {
                    await _logger.LogWarning("Invalid model state for code refresh", methodName);
                    return BadRequest(new { message = "invalid confirmation parameters", code = 400 });
                }

                await _mediator.Send(generateCodeCommand);

                await _logger.LogInfo("Confirmation code refreshed and sent successfully", methodName);
                return Ok(new { message = "email confirmation code sended to email", code = 200 });
            }
            catch (Exception ex)
            {
                await _logger.LogError("Unexpected error during code refresh", methodName, ex.Message);
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        [HttpPost("auth")]
        public async Task<IActionResult> GetTokenAsync([FromBody] GetUserByEmailAndPasswordQuery query)
        {
            string methodName = $"{nameof(UserController)}.{nameof(GetTokenAsync)}";

            try
            {
                await _logger.LogInfo("Starting user authentication", methodName);

                if (!ModelState.IsValid)
                {
                    await _logger.LogWarning("Invalid model state for authentication", methodName);
                    return BadRequest(new { message = "invalid login parameters", code = 400 });
                }

                UserDto user = await _mediator.Send(query);

                string token = _tokenService.GenerateToken(user.UserName, user.Email, user.Id, user.RoleName);

                await _logger.LogInfo($"User authenticated successfully: {user.Email}", methodName);
                return Ok(new { jwt_token = token, code = 200 });
            }
            catch (InvalidPasswordException e)
            {
                await _logger.LogWarning($"Invalid password during authentication: {e.Message}", methodName);
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (InvalidEmailException e)
            {
                await _logger.LogWarning($"Invalid email during authentication: {e.Message}", methodName);
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (NotFoundException e)
            {
                await _logger.LogWarning($"User not found during authentication: {e.Message}", methodName);
                return NotFound(new { message = e.Message });
            }
            catch (Exception ex)
            {
                await _logger.LogError("Unexpected error during authentication", methodName, ex.Message);
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        //[Authorize(User,Admin)]
        [HttpPost("update/email")]
        public async Task<IActionResult> UpdateUsersEmailAsync([FromBody] UpdateUsersEmailCommand updateUsersEmail)
        {
            string methodName = $"{nameof(UserController)}.{nameof(UpdateUsersEmailAsync)}";

            try
            {
                await _logger.LogInfo("Starting email update", methodName);

                if (!ModelState.IsValid)
                {
                    await _logger.LogWarning("Invalid model state for email update", methodName);
                    return BadRequest(new { message = "invalid update parameters", code = 400 });
                }

                await _mediator.Send(updateUsersEmail);

                await _logger.LogInfo("Email updated successfully", methodName);
                return Ok(new { message = "email updated", code = 200 });
            }
            catch (InvalidEmailException e)
            {
                await _logger.LogWarning($"Invalid email exception: {e.Message}", methodName);
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (NotFoundException e)
            {
                await _logger.LogWarning($"Not found exception: {e.Message}", methodName);
                return NotFound(new { message = e.Message });
            }
            catch (Exception ex)
            {
                await _logger.LogError("Unexpected error during email update", methodName, ex.Message);
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        //[Authorize(User,Admin)]
        [HttpPost("update/name")]
        public async Task<IActionResult> UpdateUsersNameAsync([FromBody] UpdateUsersNameCommand updateUsersName)
        {
            string methodName = $"{nameof(UserController)}.{nameof(UpdateUsersNameAsync)}";

            try
            {
                await _logger.LogInfo("Starting name update", methodName);

                if (!ModelState.IsValid)
                {
                    await _logger.LogWarning("Invalid model state for name update", methodName);
                    return BadRequest(new { message = "invalid update parameters", code = 400 });
                }

                await _mediator.Send(updateUsersName);

                await _logger.LogInfo("Name updated successfully", methodName);
                return Ok(new { message = "name updated", code = 200 });
            }
            catch (InvalidUserException e)
            {
                await _logger.LogWarning($"Invalid user exception: {e.Message}", methodName);
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (NotFoundException e)
            {
                await _logger.LogWarning($"Not found exception: {e.Message}", methodName);
                return NotFound(new { message = e.Message });
            }
            catch (Exception ex)
            {
                await _logger.LogError("Unexpected error during name update", methodName, ex.Message);
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        //[Authorize(User,Admin)]
        [HttpPost("update/password")]
        public async Task<IActionResult> UpdateUsersPasswordAsync([FromBody] UpdateUsersPasswordCommand updateUsersPassword)
        {
            string methodName = $"{nameof(UserController)}.{nameof(UpdateUsersPasswordAsync)}";

            try
            {
                await _logger.LogInfo("Starting password update", methodName);

                if (!ModelState.IsValid)
                {
                    await _logger.LogWarning("Invalid model state for password update", methodName);
                    return BadRequest(new { message = "invalid update parameters", code = 400 });
                }

                await _mediator.Send(updateUsersPassword);

                await _logger.LogInfo("Password updated successfully", methodName);
                return Ok(new { message = "password updated", code = 200 });
            }
            catch (InvalidPasswordException e)
            {
                await _logger.LogWarning($"Invalid password exception: {e.Message}", methodName);
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (NotFoundException e)
            {
                await _logger.LogWarning($"Not found exception: {e.Message}", methodName);
                return NotFound(new { message = e.Message });
            }
            catch (Exception ex)
            {
                await _logger.LogError("Unexpected error during password update", methodName, ex.Message);
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        //[Authorize(Admin)]
        [HttpPost("activate/{id:guid}")]
        public async Task<IActionResult> UpdateUsersActiveStateAsync(Guid id)
        {
            string methodName = $"{nameof(UserController)}.{nameof(UpdateUsersActiveStateAsync)}";

            try
            {
                await _logger.LogInfo($"Starting user activation/deactivation for ID: {id}", methodName);

                ActivateUserCommand activateUserCommand = new ActivateUserCommand();
                activateUserCommand.UserId = id;

                await _mediator.Send(activateUserCommand);

                await _logger.LogInfo($"User active state changed for ID: {id}", methodName);
                return Ok(new { message = "user active state changed", code = 200 });
            }
            catch (InvalidUserException e)
            {
                await _logger.LogWarning($"Invalid user exception: {e.Message}", methodName);
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (NotFoundException e)
            {
                await _logger.LogWarning($"Not found exception: {e.Message}", methodName);
                return NotFound(new { message = e.Message });
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Unexpected error during user activation for ID: {id}", methodName, ex.Message);
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        //[Authorize(User,Admin)]
        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> DeleteUserAsync(Guid id)
        {
            string methodName = $"{nameof(UserController)}.{nameof(DeleteUserAsync)}";

            try
            {
                await _logger.LogInfo($"Starting user deletion for ID: {id}", methodName);

                DeleteUserCommand deleteUserCommand = new DeleteUserCommand();
                deleteUserCommand.UserId = id;

                await _mediator.Send(deleteUserCommand);

                await _logger.LogInfo($"User deleted successfully for ID: {id}", methodName);
                return Ok(new { message = "user deleted", code = 200 });
            }
            catch (InvalidUserException e)
            {
                await _logger.LogWarning($"Invalid user exception: {e.Message}", methodName);
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (NotFoundException e)
            {
                await _logger.LogWarning($"Not found exception: {e.Message}", methodName);
                return NotFound(new { message = e.Message });
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Unexpected error during user deletion for ID: {id}", methodName, ex.Message);
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        //[Authorize(User,Admin)]
        [HttpGet("get/all")]
        public async Task<IActionResult> GetUsersAsync()
        {
            string methodName = $"{nameof(UserController)}.{nameof(GetUsersAsync)}";

            try
            {
                await _logger.LogInfo("Getting all users", methodName);

                List<UserDto> users = await _mediator.Send(new GetUsersQuery());

                await _logger.LogInfo($"Retrieved {users.Count} users successfully", methodName);
                return Ok(users);
            }
            catch (Exception ex)
            {
                await _logger.LogError("Unexpected error while getting all users", methodName, ex.Message);
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        //[Authorize(Admin)]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserByIdAsync(Guid id)
        {
            string methodName = $"{nameof(UserController)}.{nameof(GetUserByIdAsync)}";

            try
            {
                await _logger.LogInfo($"Getting user by ID: {id}", methodName);

                GetUserByIdQuery query = new GetUserByIdQuery();
                query.Id = id;

                UserDto user = await _mediator.Send(query);

                await _logger.LogInfo($"User retrieved successfully for ID: {id}", methodName);
                return Ok(user);
            }
            catch (NotFoundException e)
            {
                await _logger.LogWarning($"User not found for ID: {id}", methodName);
                return NotFound(new { message = e.Message });
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Unexpected error while getting user by ID: {id}", methodName, ex.Message);
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }
    }
}
