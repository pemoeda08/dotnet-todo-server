using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoServer.Data;
using TodoServer.Data.Entities;
using TodoServer.Data.Impl;
using TodoServer.Dto.Account;
using TodoServer.Dto.User;

namespace TodoServer.Controllers
{
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository userRepo;
        private readonly IMapper mapper;

        public AccountController(IUserRepository userRepo, IMapper mapper)
        {
            this.userRepo = userRepo;
            this.mapper = mapper;
        }

        [HttpPut]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto registerDto)
        {
            bool alreadyExists = await userRepo.Exists(registerDto.Username);
            if (alreadyExists)
                return Problem(
                    detail: "username already exists.",
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