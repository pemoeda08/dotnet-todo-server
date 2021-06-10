using Microsoft.AspNetCore.Mvc;
using TodoServer.Dto;
using TodoServer.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace TodoServer.Controllers
{

    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository userRepo;

        public AuthController(IUserRepository userRepo)
        {
            this.userRepo = userRepo;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AuthDto login)
        {
            var user = await userRepo.GetUser(login.Username, login.Password);
            if (user == null)
                return Unauthorized();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("hello_world_hello_world"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var exp = DateTime.Now.AddMinutes(5);
            var token = new JwtSecurityToken(
                issuer: "TodoServer",
                audience: "TodoClient",
                claims: new Claim[] {
                    new Claim("id", user.Id.ToString()),
                    new Claim("username", login.Username)
                },
                expires: exp,
                signingCredentials: creds
            );
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("authorized");
        }
    }
}