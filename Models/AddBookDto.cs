using System.ComponentModel;

namespace LibraryManagementSystem.Models
{
    public class AddBookDto
    {
        [DefaultValue("")]
        public required string Title { get; set; }
        [DefaultValue("")]
        public string Author { get; set; }
        [DefaultValue("")]
        public string ISBN { get; set; }
        [DefaultValue(null)]
        public DateTime? PublicationYear { get; set; }
        [DefaultValue("")]
        public string Genre { get; set; }

       // [DefaultValue(int.MinValue)]
        public int Quantity { get; set; }
    }
}
