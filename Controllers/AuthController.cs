using Microsoft.AspNetCore.Mvc;
using TodoServer.Dto;
using TodoServer.Data;
using System.Threading.Tasks;

namespace TodoServer.Controllers
{

    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository userRepo;

        public AuthController(IUserRepository userRepo)
        {
            this.userRepo = userRepo;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AuthDto login)
        {
            var user = await userRepo.GetUser(login.Username, login.Password);
            if (user == null)
                return Unauthorized();

            return Ok();
        }
    }
}