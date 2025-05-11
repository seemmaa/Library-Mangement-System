using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services.Interfaces;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowingsController: ControllerBase
    {
        private readonly IBorrowing _borrowingService;
        public BorrowingsController(IBorrowing borrowingService)
        {
            _borrowingService = borrowingService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllBorrowings()
        {
            var borrowings = await _borrowingService.GetAllBorrowingsAsync();
            return Ok(borrowings);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBorrowingById(Guid id)
        {
            var borrowing = await _borrowingService.GetBorrowingByIdAsync(id);
            if (borrowing == null)
            {
                return NotFound();
            }
            return Ok(borrowing);
        }
        [Authorize(Roles ="Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateBorrowing([FromBody]Borrowing borrowing)
        {
            if (borrowing == null)
            {
                return BadRequest("Borrowing cannot be null.");
            }
            var updatedBorrowing = await _borrowingService.UpdateBorrowingAsync(borrowing);
            if (updatedBorrowing == null)
            {
                return NotFound();
            }
            return Ok(updatedBorrowing);
        }
        [Authorize(Roles = "Member")]
        [HttpGet("borrowing-history")]
        public async Task<IActionResult> GetBorrowingHistory()
        {
            var borrowings = await _borrowingService.GetBorrowingHistoryAsynch();
            if (borrowings == null)
            {
                return NotFound();
            }
            return Ok(borrowings);
        }
        [Authorize(Roles ="Member")]
        [HttpGet("active-borrowings")]
        public async Task<IActionResult> GetActiveBorrowings()
        {
            var borrowings = await _borrowingService.GetCurrentBorrowingsAsync();
            if (borrowings == null)
            {
                return NotFound();
            }
            return Ok(borrowings);
        }
        [Authorize(Roles ="Member")]
        [HttpPost("borrow")]
        public async Task<IActionResult> BorrowBook(Guid bookId)

        {
            var result=await _borrowingService.BorrowBookAsync(bookId);
            return Ok(result);
        }
        [Authorize(Roles = "Member")]
        [HttpPut("return")]
        public async Task<IActionResult> ReturnBook(Guid borrowingId)
        {
            var result = await _borrowingService.ReturnBookAsync(borrowingId);
            return Ok(result);
        }
    }
    
}
