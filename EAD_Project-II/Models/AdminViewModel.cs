using System.ComponentModel.DataAnnotations;

namespace EAD_Project_II.Models
{
    public class AdminViewModel
    {
        [Required(ErrorMessage = "Please enter your name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter your password")]
        public string? Password { get; set; }
    }
}
