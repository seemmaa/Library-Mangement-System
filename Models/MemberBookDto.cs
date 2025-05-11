namespace LibraryManagementSystem.Models
{
    public class MemberBookDto
    {
       public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Genre { get; set; }

        public DateTime? PublicationYear { get; set; }
        public int Quantity { get; set; }
    }
}
