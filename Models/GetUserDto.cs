namespace LibraryManagementSystem.Models
{
    public class GetUserDto
    {
        public string Email { get; set; }
        public string Role { get; set; } // e.g., "Admin", "Member"
        public bool IsActive { get; set; }
        public Guid Id { get; set; }
    }
}
