using LibrarySystem.Business.BorrowBusiness;
using LibrarySystem.Shared.BorrowData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin,Staff")]
    public class BorrowController : ControllerBase
    {
        private readonly IBorrowBusiness _borrowBusiness;
        private readonly IConfiguration _configuration;

        public BorrowController(IBorrowBusiness borrowBusiness, IConfiguration configuration)
        {
            _borrowBusiness = borrowBusiness;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var borrows = await _borrowBusiness.GetBorrowList();
            return Ok(borrows);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var borrow = await _borrowBusiness.GetBorrowDetails(id);
            if (borrow == null)
            {
                return NotFound(new { message = $"Borrow record with ID {id} not found." });
            }
            return Ok(borrow);
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateBorrowParams([FromQuery] int memberId, [FromQuery] int bookId)
        {
            var (isValid, errorMessage) = await _borrowBusiness.ValidateBorrow(memberId, bookId);
            return Ok(new { isValid, errorMessage });
        }

        [HttpPost]
        public async Task<IActionResult> BorrowBook([FromBody] BorrowRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate borrow eligibility
            var (isValid, errorMessage) = await _borrowBusiness.ValidateBorrow(request.MemberId, request.BookId);
            if (!isValid)
            {
                return BadRequest(new { message = errorMessage });
            }

            var borrowDetails = new BorrowDetails
            {
                BookId = request.BookId,
                MemberId = request.MemberId,
                BorrowedOn = DateTime.UtcNow,
                Status = "Active",
                User = User.Identity?.Name ?? "api-operator"
            };

            // Calculate due date based on configuration
            int dueDateDays = _configuration.GetValue<int>("LibrarySettings:DueDateDays", 15);
            borrowDetails.DueDate = borrowDetails.BorrowedOn.AddDays(dueDateDays);

            var success = await _borrowBusiness.AddBorrow(borrowDetails);
            if (success)
            {
                return Ok(new { message = "Book borrowed successfully.", dueDate = borrowDetails.DueDate });
            }
            return BadRequest(new { message = "Failed to process borrow transaction." });
        }

        [HttpPost("{id}/return")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            decimal fineAmountPerDay = _configuration.GetValue<decimal>("LibrarySettings:FineAmountPerDay", 5);
            var success = await _borrowBusiness.ReturnBook(id, fineAmountPerDay);
            if (success)
            {
                return Ok(new { message = "Book returned successfully." });
            }
            return BadRequest(new { message = "Failed to process book return." });
        }
    }

    public class BorrowRequestDto
    {
        public int MemberId { get; set; }
        public int BookId { get; set; }
    }
}
