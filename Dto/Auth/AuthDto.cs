using System.ComponentModel.DataAnnotations;

namespace TodoServer.Dto
{
    public class AuthDto
    {

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}