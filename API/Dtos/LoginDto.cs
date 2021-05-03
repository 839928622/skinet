using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email address is required")]
        public string Email { get; init; }
        [Required(ErrorMessage = "Password address is required")]
        public string Password { get; init; }
    }
}
