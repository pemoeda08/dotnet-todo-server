using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using TodoServer.Data;
using TodoServer.Data.Entities;
using TodoServer.Data.Impl;
using TodoServer.Dto.Account;
using TodoServer.Dto.User;

namespace TodoServer.Controllers
{
    
    [Route("account")]
    [ApiController]
    [SwaggerTag("Manage account")]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository userRepo;
        private readonly IMapper mapper;

        public AccountController(IUserRepository userRepo, IMapper mapper)
        {
            this.userRepo = userRepo;
            this.mapper = mapper;
        }

        [SwaggerOperation(
            Summary = "Create new user/account"
        )]
        [SwaggerResponse(200, "Returns newly created user", typeof(UserDto))]
        [SwaggerResponse(409, "Username is already used")]
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateAccount([FromBody] CreateAccountDto registerDto)
        {
            bool alreadyExists = await userRepo.Exists(registerDto.Username);
            if (alreadyExists)
                return Problem(
                    title: $"username '{registerDto.Username}' already exists.",
                    statusCode: StatusCodes.Status409Conflict
                );

            var newUser = await userRepo.CreateUser(new User
            {
                HashedPassword = registerDto.Password,
                Username = registerDto.Username
            });

            var newUserDto = mapper.Map<UserDto>(newUser);
            return Created(string.Empty, newUserDto);
        }
    }
}