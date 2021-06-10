using System.ComponentModel.DataAnnotations;

namespace TodoServer.Dto.Account
{
    public class CreateAccountDto
    {

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

    }
}