namespace LibraryManagementSystem.Models
{
    public class BorrowingHistoryDto
    {
        public Guid Id { get; set; }
        public BookDto Book { get; set; }

        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsReturned { get; set; }
        public bool IsOverdue => DueDate < DateTime.Now && !IsReturned;
    }
}
