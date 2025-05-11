using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IBorrowing
    {
        public Task<List<Borrowing>> GetAllBorrowingsAsync();
        public Task<Borrowing> GetBorrowingByIdAsync(Guid id);
        public Task<string> BorrowBookAsync(Guid bookId);
        public Task<string> ReturnBookAsync(Guid borrowingId);
        public Task<string> UpdateBorrowingAsync(Borrowing borrowing);
        public Task<List<BorrowingHistoryDto>> GetCurrentBorrowingsAsync();
        public Task<List<BorrowingHistoryDto>> GetBorrowingHistoryAsynch();
       

    }
}
