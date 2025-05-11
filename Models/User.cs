using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace LibraryManagementSystem.Models
{
    public class User: IdentityUser<Guid>
    {
        public Guid Id { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }
        public string Role { get; set; } // e.g., "Admin", "Member"
        public bool IsActive { get; set; }
        // Navigation property for the borrowed books
        [JsonIgnore]
        public ICollection<Borrowing> BorrowRecords { get; set; }
    }
}
