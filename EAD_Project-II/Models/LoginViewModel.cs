using System.ComponentModel.DataAnnotations;
namespace EAD_Project_II.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your email")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
