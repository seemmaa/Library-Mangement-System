using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using LibraryManagementSystem.Services.Interfaces;
namespace LibraryManagementSystem.Services
{
    public class BorrowingService:IBorrowing
    {
        private readonly LibraryContext _context;
        private readonly ILogger<BorrowingService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
       // private readonly IConfiguration _configuration;
        public BorrowingService(LibraryContext context, IHttpContextAccessor httpContextAccessor,
            ILogger<BorrowingService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            //_configuration = configuration;
            //var app = _configuration.GetSection("AppSettings");
        }
        public async Task<List<Borrowing>> GetAllBorrowingsAsync()
        {
            return await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .ToListAsync();
        }
        public async Task<Borrowing> GetBorrowingByIdAsync(Guid id)
        {
            

           var result =await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);
            if(result==null)
            {
                return null;
            }
            return result;
        }
        public async Task<string> BorrowBookAsync(Guid bookId)
        {
            var id =  _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = Guid.Parse(id);

            var user = await _context.Users.FindAsync(userId);
            if (user.IsActive == false)
            {
                _logger.LogInformation($"User {user.Email} is not active.");
                return "User is not active.";
            }


            var book = await _context.Books.FindAsync(bookId);
            if (book == null) {
                _logger.LogWarning($"Book with ID {bookId} not found.");
                return "Book not found.";
               
            }
            var userr = await _context.Users.FindAsync(userId);
            if (userr == null) {
                _logger.LogWarning($"User with ID {userId} not found.");
                return "User not found."; }
            if (book.AvailableCopies == 0)
            {
                _logger.LogWarning($"Book with ID {bookId} is out of stock.");
                return "Book out of stock.";
            }

            bool alreadyBorrowed = await _context.Borrowings
                .AnyAsync(b => b.BookId == bookId && b.UserId == userId && b.IsReturned==false);

            if (alreadyBorrowed)
            {
                _logger.LogWarning($"User {user.Email} has already borrowed book {book.Title}.");
                return "You have already borrowed this book and haven't returned it yet.";
            }
            int activeBorrowCount = await _context.Borrowings
                .CountAsync(b => b.UserId == userId && b.ReturnDate == DateTime.MinValue);

            if (activeBorrowCount >= 3)
            {
                _logger.LogWarning($"User {user.Email} has reached the borrowing limit.");
                return "You can't borrow more than 3 books.";
            }
           

            var borrowing = new Borrowing
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                UserId = userId,
                BorrowDate = DateTime.Now,
                ReturnDate = DateTime.MinValue
            };

            _context.Borrowings.Add(borrowing);
            book.AvailableCopies--;
            book.TimesBorrowed++;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Book {book.Title} borrowed by user {userr.Email}.");

            return "Book borrowed successfully.";
        }





        public async Task<string> UpdateBorrowingAsync(Borrowing borrowing)
        {if (_context.Borrowings.FirstOrDefault(a => a.Id == borrowing.Id) == null)
                return null;
            _context.Borrowings.Update(borrowing);
            await _context.SaveChangesAsync();
            return "Borrowing updated successfully.";
        }
        public async Task DeleteBorrowingAsync(Guid id)
        {
            var borrowing = await GetBorrowingByIdAsync(id);
            if(borrowing==null)
            {
                _logger.LogWarning($"Borrowing record with ID {id} not found.");
                return;
            }
            
                _context.Borrowings.Remove(borrowing);
                await _context.SaveChangesAsync();
           
        }

        public async Task<string> ReturnBookAsync(Guid id)
        {
            var borrowing = await GetBorrowingByIdAsync(id);
            if (borrowing == null)
            {
                _logger.LogWarning($"Borrowing record with ID {id} not found.");
                return "Borrowing record not found.";
            }
            if (borrowing.IsReturned)
            {
                _logger.LogWarning($"Book with ID {borrowing.BookId} already returned.");
                return "This book has already been returned.";
            }
           
            var book = await _context.Books.FindAsync(borrowing.BookId);
           //if (book.AvailableCopies >= book.Quantity) return "All copies are already in stock";
            if (book != null)
            {
                book.AvailableCopies++;
                borrowing.IsReturned = true;
                borrowing.ReturnDate = DateTime.Now;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Book with ID {borrowing.BookId} returned successfully.");
                return "Book returned successfully.";
            }
            _logger.LogWarning($"Book with ID {borrowing.BookId} not found.");
            return "Book not found.";
        }
        public async Task<List<BorrowingHistoryDto>> GetCurrentBorrowingsAsync()
        {
            var id = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = Guid.Parse(id);

            var result= await _context.Borrowings
                .Include(b => b.Book)
                .Where(b => b.UserId == userId)
                .ToListAsync();
            result = result.Where(b => b.IsReturned==false).ToList();

            var currentBorrowing=result.Select(b=> new BorrowingHistoryDto
            {
                Id = b.Id,
               
                BorrowDate = b.BorrowDate,
                ReturnDate = b.ReturnDate,
                DueDate = b.DueDate,
                IsReturned = b.IsReturned,
               Book = new BookDto
               {
                   Id = b.Book.Id,
                   Title = b.Book.Title,
                   Author = b.Book.Author,
                   Genre = b.Book.Genre,
                   ISBN = b.Book.ISBN,
                   PublicationYear = b.Book.PublicationYear,
                   Quantity = b.Book.Quantity
               }
            }).ToList();
            return currentBorrowing;

        }
        public async Task<List<BorrowingHistoryDto>> GetBorrowingHistoryAsynch()
        {
            var id = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = Guid.Parse(id);
            var result = await _context.Borrowings
                .Include(b => b.Book)
                .Where(b => b.UserId == userId)
                .ToListAsync();
            var BorrowingHistory = result.Select(b => new BorrowingHistoryDto
            {
                Id = b.Id,
                BorrowDate = b.BorrowDate,
                ReturnDate = b.ReturnDate,
                DueDate = b.DueDate,
                IsReturned = b.IsReturned,
                Book = new BookDto
                {
                    Id = b.Book.Id,
                    Title = b.Book.Title,
                    Author = b.Book.Author,
                    Genre = b.Book.Genre,
                    ISBN = b.Book.ISBN,
                    PublicationYear = b.Book.PublicationYear,
                    Quantity = b.Book.Quantity
                }
            }).ToList();

            return BorrowingHistory;
        }
    }
}
