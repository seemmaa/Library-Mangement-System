using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Services.Interfaces
{
    public interface IBook
    {
        public Task<List<Book>> GetAllBooksAsync();
        public Task<List<Book>> GetCatalog();
        public Task<Book> GetBookByIdAsync(Guid id);
        public Task AddBookAsync(AddBookDto book);

        public Task<string> UpdateBookAsync(Guid id, AddBookDto book);
        public Task<string> DeleteBookAsync(Guid id);
        public Task<List<MemberBookDto>> MemberSearchBooksAsync(string searchTerm);
        public  Task<List<Book>> SearchBooksAsync(string searchTerm);

    }
}
