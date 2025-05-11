using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBook _bookService;
        public BooksController(IBook bookService)
        {
            _bookService = bookService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }
        [Authorize(Roles = "Member")]
        [HttpGet("Catalog")]
        public async Task<IActionResult> GetCatalog()
        {
            var catalog = await _bookService.GetCatalog();
            return Ok(catalog);
        }
        [Authorize(Roles = "Member")]
        [HttpGet("member-search")]
        public async Task<IActionResult> GetMemberSearch(string searchTerm)
        {
            var books = await _bookService.MemberSearchBooksAsync(searchTerm);
            return Ok(books);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody]AddBookDto book)
        {
            if (book == null)
            {
                return BadRequest("Book cannot be null.");
            }
            await _bookService.AddBookAsync(book);
            return Ok(book);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, [FromBody] AddBookDto book)
        {
           
            var result= await _bookService.UpdateBookAsync(id,book);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var result =await _bookService.DeleteBookAsync(id);
           
            return Ok(result);
        }
        [Authorize(Roles = "Admin, Member")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks(string searchTerm)
        {
            var books = await _bookService.SearchBooksAsync(searchTerm);
            return Ok(books);
        }
    }
}
