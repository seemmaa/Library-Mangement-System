using System.ComponentModel;
using System.Text.Json.Serialization;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public enum BookStatus
        {
            Available,
            Borrowed,
            Overdue
        }
        public Guid Id { get; set; }
        [DefaultValue("")]
        public required string Title { get; set; }
        [DefaultValue("")]
        public string Author { get; set; }
        [DefaultValue("")]
        public string ISBN { get; set; }
     
        public DateTime? PublicationYear { get; set; }

        public string Genre { get; set; }


        [DefaultValue(null)]
        public int Quantity { get; set; }

        public int AvailableCopies { get; set; }

        public int TimesBorrowed { get; set; }
        [JsonIgnore]
        public ICollection<Borrowing> BorrowRecords { get; set; }
    }
}
