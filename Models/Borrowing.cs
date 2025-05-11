using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace LibraryManagementSystem.Models
{
    public class Borrowing
    {
        public Guid Id { get; set; }

        public Guid BookId { get; set; }
        public Guid UserId { get; set; }

        public DateTime BorrowDate { get; set; } = DateTime.Now;
        public DateTime ReturnDate { get; set; }

        public DateTime DueDate { get;  set; } // Add private set

        public bool IsReturned { get; set; }

        public bool IsOverdue => DueDate < DateTime.Now && !IsReturned;

        // Navigation properties
        [JsonIgnore]
        [ForeignKey("BookId")]
        public Book Book { get; set; }
        [JsonIgnore]
        [ForeignKey("UserId")]
        public User User { get; set; }

        // Constructor to auto-calculate DueDate
        public Borrowing()
        {
            DueDate = BorrowDate.AddDays(14);
        }
    }
}
