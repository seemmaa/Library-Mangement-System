using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IAuth
    {
        public Task<(bool IsSuccess, string Message)> SignupAsync(RegisterDto register);
        public Task<(bool IsSuccess, string TokenOrMessage)> LoginAsync(string email, string password);
       // private string GenerateJwtToken(IdentityUser user, IList<string> roles);
    }
}
