using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
namespace LibraryManagementSystem.Models
{
    public class LibraryContext : DbContext,LibraryDbInterface
       
    { public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users{ get; set; }
        public DbSet<Borrowing> Borrowings { get; set; }
       
       
    }
}
