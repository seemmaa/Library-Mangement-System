using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LibraryManagementSystem.Services
{
    public class BookService:IBook
    {
        private readonly LibraryContext _context;
        private readonly ILogger<BookService> _logger;
        public BookService(LibraryContext context, ILogger<BookService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<Book>> GetAllBooksAsync()
        {
            return await _context.Books.ToListAsync();
        }
        public async Task<List<Book>> GetCatalog()
        {
            var result=await _context.Books.ToListAsync();
            var catalog = result.Where(b => b.Quantity > 0).ToList();
            return catalog;
        }
        public async Task<Book> GetBookByIdAsync(Guid id)
        {
            return await _context.Books.FindAsync(id);
        }
        public async Task AddBookAsync(AddBookDto book)
        {
            var newBook = new Book
            {
                Id = Guid.NewGuid(),
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                PublicationYear = book.PublicationYear,
                ISBN = book.ISBN,
                Quantity = book.Quantity
            };
            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Book {newBook.Title} added successfully.");
        }
        public async Task<string> UpdateBookAsync(Guid id, AddBookDto book)
        {
            var updateBook = await GetBookByIdAsync(id);
            if (updateBook == null)
            {
                _logger.LogWarning($"Book with ID {id} not found.");
                return "book not found";
            }
           
            if (!book.Title.Equals(""))
                updateBook.Title= book.Title;
            if (!book.Author.Equals(""))
                updateBook.Author= book.Author;
            if (!book.Genre.Equals(""))
                updateBook.Genre = book.Genre;
            if(!book.ISBN.Equals(""))
                updateBook.ISBN= book.ISBN;
            if (book.PublicationYear.HasValue)
                updateBook.PublicationYear=book.PublicationYear;
            if(book.Quantity!= updateBook.Quantity)
                updateBook.Quantity= book.Quantity;
           

            _context.Books.Update(updateBook);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Book {book.Title} updated successfully.");
            return "Book updated successfully";
        }
        public async Task<string> DeleteBookAsync(Guid id)
        {
            var book = await GetBookByIdAsync(id);
            if(book == null)
            {
                _logger.LogWarning($"Book with ID {id} not found.");
                return "Book not found";
            }
            if (_context.Borrowings.Any(r=>r.BookId==book.Id)==true)
            {
                _logger.LogWarning($"Book with ID {id} cannot be deleted as it is currently borrowed.");
                return "Book cannot be deleted as it is currently borrowed";
            }
           
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            _logger.LogInformation($"Book with ID {id} deleted successfully.");

            return "Book deleted successfully";
        }

        public async Task<List<Book>> SearchBooksAsync(string searchTerm)
        {
            return await _context.Books
                .Where(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm)|| b.Genre.Contains(searchTerm))
                .ToListAsync();
        }
        public async Task<List<MemberBookDto>> MemberSearchBooksAsync(string searchTerm)
        {
           var result=  await _context.Books
                .Where(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm) || b.Genre.Contains(searchTerm))
                .ToListAsync();

            var catalog = result.Where(b => b.Quantity > 0).ToList();
            var searchResult=catalog.Select(result => new MemberBookDto
            {
                
                Title = result.Title,
                Author = result.Author,
                Genre = result.Genre,
                ISBN = result.ISBN,
                PublicationYear = result.PublicationYear,
                Quantity = result.Quantity
            }).ToList();
            return searchResult;
        }
    }
}
