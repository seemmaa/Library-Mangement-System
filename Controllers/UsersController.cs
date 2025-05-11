using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using LibraryManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUser _userService;
        public UsersController(IUser userService)
        {
            _userService = userService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllMembers()
        {
            var members = await _userService.GetAllMembersAsync();
            return Ok(members);
        }
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetMemberById(Guid id)
        //{
        //    var member = await _userService.GetMemberByIdAsync(id);
        //    if (member == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(member);
        //}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/activate-member")]
        //public async Task<IActionResult> UpdateMemberStatus(Guid id, [FromBody] StatusUpdateDto statusDto)
        //{
        //    var result = await _userService.UpdateStatusAsync(id, statusDto.IsActive);
        //    return Ok(result);
        //}
        
        public async Task<IActionResult> ActivateMember(Guid id)
        {
            var result = await _userService.ActivateMember(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/deactivate-member")]
        public async Task<IActionResult> DeactivateMember(Guid id)
        {
            
            var result = await _userService.DeactivateMember(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }



        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/borrowings")]

        public async Task<IActionResult> GetMemberBorrowings(Guid id)
        {
            var borrowings = await _userService.GetBorrowingsByMemberIdAsync(id);
            if (borrowings == null)
            {
                return NotFound();
            }
            return Ok(borrowings);
        }


    }
}
