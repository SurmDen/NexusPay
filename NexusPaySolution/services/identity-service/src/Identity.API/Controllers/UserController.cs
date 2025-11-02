using Identity.Application.Interfaces;
using Identity.Application.User.Commands;
using Identity.Application.User.DTOs;
using Identity.Application.User.Queries;
using Identity.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [ApiController]
    [Route("api/v1/user")]
    public class UserController : ControllerBase
    {
        public UserController(IMediator mediator, ITokenService tokenService)
        {
            _mediator = mediator;
            _tokenService = tokenService;
        }

        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;

        [HttpPost("create")]
        public async Task<IActionResult> CreateUserAsync([FromBody] RegisterUserCommand registerUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "invalid register parameters", code = 400 });
            }

            try
            {
                Guid userId = await _mediator.Send(registerUser);

                return Ok(new { message = "user created", user_id = userId, code = 200 });
            }
            catch (InvalidPasswordException e)
            {
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (InvalidUserException e)
            {
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (InvalidEmailException e)
            {
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (NotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception)
            {
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        [HttpPost("auth")]
        public async Task<IActionResult> GetTokenAsync([FromBody] GetUserByEmailAndPasswordQuery query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "invalid login parameters", code = 400 });
            }

            try
            {
                UserDto user = await _mediator.Send(query);

                string token = _tokenService.GenerateToken(user.UserName, user.Email, user.Id);

                return Ok(new { jwt_token = token, code = 200 });
            }
            catch (InvalidPasswordException e)
            {
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (InvalidEmailException e)
            {
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (NotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception)
            {
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        [HttpPost("update/email")]
        public async Task<IActionResult> UpdateUsersEmailAsync([FromBody] UpdateUsersEmailCommand updateUsersEmail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "invalid update parameters", code = 400 });
            }

            try
            {
                await _mediator.Send(updateUsersEmail);

                return Ok(new { message = "email updated", code = 200 });
            }
            catch (InvalidEmailException e)
            {
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (NotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception)
            {
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        [HttpPost("update/name")]
        public async Task<IActionResult> UpdateUsersNameAsync([FromBody] UpdateUsersNameCommand updateUsersName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "invalid update parameters", code = 400 });
            }

            try
            {
                await _mediator.Send(updateUsersName);

                return Ok(new { message = "name updated", code = 200 });
            }
            catch (InvalidUserException e)
            {
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (NotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception)
            {
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        [HttpPost("update/password")]
        public async Task<IActionResult> UpdateUsersPasswordAsync([FromBody] UpdateUsersPasswordCommand updateUsersPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "invalid update parameters", code = 400 });
            }

            try
            {
                await _mediator.Send(updateUsersPassword);

                return Ok(new { message = "password updated", code = 200 });
            }
            catch (InvalidPasswordException e)
            {
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (NotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception)
            {
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        [HttpPost("activate/{id:guid}")]
        public async Task<IActionResult> UpdateUsersActiveStateAsync(Guid id)
        {
            try
            {
                ActivateUserCommand activateUserCommand = new ActivateUserCommand();

                activateUserCommand.UserId = id;

                await _mediator.Send(activateUserCommand);

                return Ok(new { message = "user active state changed", code = 200 });
            }
            catch (InvalidUserException e)
            {
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (NotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception)
            {
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> DeleteUserAsync(Guid id)
        {
            try
            {
                DeleteUserCommand deleteUserCommand = new DeleteUserCommand();

                deleteUserCommand.UserId = id;

                await _mediator.Send(deleteUserCommand);

                return Ok(new { message = "user deleted", code = 200 });
            }
            catch (InvalidUserException e)
            {
                return BadRequest(new { message = e.Message, code = 400 });
            }
            catch (NotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception)
            {
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        [HttpGet("get/all")]
        public async Task<IActionResult> GetUsersAsync()
        {
            try
            {
               List<UserDto> users = await _mediator.Send(new GetUsersQuery());

               return Ok(users);
            }
            catch (Exception)
            {
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserByIdAsync(Guid id)
        {
            try
            {
                GetUserByIdQuery query = new GetUserByIdQuery();

                query.Id = id;

                UserDto user = await _mediator.Send(query);

                return Ok(user);
            }
            catch (NotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception)
            {
                return Problem(title: "Internal server error", statusCode: 500);
            }
        }
    }
}
