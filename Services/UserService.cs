using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace LibraryManagementSystem.Services
{
    public class UserService :IUser
    {
        private readonly LibraryContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserService> _logger;

        public UserService(LibraryContext context, RoleManager<IdentityRole> roleManager, ILogger<UserService> logger)
        {
            _context = context;
            _roleManager = roleManager;
            _logger = logger;
        }
        public async Task<List<User>> GetAllMembersAsync()
        {
            var result = await _context.Users.ToListAsync();
            var members = result.Where(m => m.Role == "Member").ToList();
           
            return members;
        }
        public async Task<User> GetMemberByIdAsync(Guid id)
        {
            var member = await _context.Users.FindAsync(id);
            if (member == null)
            {
               
                return null;
            }
            
            var result = await _context.Users.ToListAsync();
            var members = result.Where(m => m.Role == "Member").ToList();
            return members.FirstOrDefault(n => n.Id == id);
        }

        //public async Task AddMemberAsync(User user)
        //{
        //    var newUser = new User
        //    {
        //        Id = Guid.NewGuid(),
        //        Email = user.Email,
        //        Password = user.Password,
        //        Role = "Member",
        //        IsActive = true
        //    };
        //    _context.Users.Add(newUser);
        //    await _context.SaveChangesAsync();
        //}
        public async Task<string> DeactivateMember(Guid id)
        {
            var member = await GetMemberByIdAsync(id);
            if (member.IsActive == false)
            {
                return ("Member is already deactivated.");
            }
            if (member == null)
            {
                return ("Member not found.");
            }
            member.IsActive = false;
            _context.Users.Update(member);
            await _context.SaveChangesAsync();
            return ("Member deactivated successfully.");

        }

        public async Task<string> ActivateMember(Guid id)
        {
            var member = await GetMemberByIdAsync(id);
            if (member.IsActive == true)
            {
                return ("Member is already activated.");
            }
            if (member == null)
            {
                return ("Member not found.");
            }
            member.IsActive = true;
            _context.Users.Update(member);
            await _context.SaveChangesAsync();
            return ("Member activated successfully.");
        }

        //public async Task<string> UpdateStatusAsync(Guid id, bool isActive)
        //{
        //    var member = await GetMemberByIdAsync(id);
        //    if (member == null)
        //    {
        //        _logger.LogWarning($"Member with ID {id} not found.");
        //        return "Member not found";
        //    }
        //    member.IsActive = isActive;
        //    _context.Users.Update(member);
        //    await _context.SaveChangesAsync();
        //    _logger.LogInformation($"Member with ID {id} status updated to {(bool)isActive}");
        //    return "Member status updated successfully";

        //}

        public async Task<ICollection<Borrowing>> GetBorrowingsByMemberIdAsync(Guid memberId)
        {
            var member = await _context.Users.FindAsync(memberId);
            if (member==null)
            { return null; }
            var borrowings = await _context.Borrowings
                .Include(b => b.Book)
                .Where(b => b.UserId == memberId)
                .ToListAsync();
            var result =borrowings.Select(borrowings => new BorrowingHistoryDto
            {
                Id = borrowings.Id,
               
                BorrowDate = borrowings.BorrowDate,
                ReturnDate = borrowings.ReturnDate,
                Book = new BookDto

                {
                    Id = borrowings.Book.Id,
                    Title = borrowings.Book.Title,
                    Author = borrowings.Book.Author,
                    Genre = borrowings.Book.Genre,
                    ISBN = borrowings.Book.ISBN,
                    PublicationYear = borrowings.Book.PublicationYear,
                    Quantity = borrowings.Book.Quantity
                }
            }).ToList();
            
            return borrowings;
        }
    }
}
