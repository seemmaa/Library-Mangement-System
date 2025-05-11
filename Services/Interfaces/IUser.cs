using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IUser
    {
        public Task<List<User>> GetAllMembersAsync();
        public Task<User> GetMemberByIdAsync(Guid id);
       // public Task<string> UpdateStatusAsync(Guid id, bool isActive);
        public Task<ICollection<Borrowing>> GetBorrowingsByMemberIdAsync(Guid id);
        public Task<string> DeactivateMember(Guid id);
        public Task<string> ActivateMember(Guid id);

    }
}
