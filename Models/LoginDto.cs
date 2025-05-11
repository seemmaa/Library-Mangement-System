using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibraryManagementSystem.Models
{
    public class LoginDto
    {
        [EmailAddress]
        public string Email { get; set; }
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}
