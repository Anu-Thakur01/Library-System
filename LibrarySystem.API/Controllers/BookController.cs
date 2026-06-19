using LibrarySystem.Business.BookBusiness;
using LibrarySystem.Shared.BookData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BookController : ControllerBase
    {
        private readonly IBookBusiness _bookBusiness;

        public BookController(IBookBusiness bookBusiness)
        {
            _bookBusiness = bookBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var books = await _bookBusiness.GetBookList();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var book = await _bookBusiness.GetBookDetails(id);
            if (book == null)
            {
                return NotFound(new { message = $"Book with ID {id} not found." });
            }
            return Ok(book);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] BookDetails book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            book.User = User.Identity?.Name ?? "api-admin";
            var success = await _bookBusiness.AddBook(book);
            if (success)
            {
                return Ok(new { message = "Book created successfully." });
            }
            return BadRequest(new { message = "Failed to create book." });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] BookDetails book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            book.BookId = id;
            book.User = User.Identity?.Name ?? "api-admin";
            var success = await _bookBusiness.EditBooks(book);
            if (success)
            {
                return Ok(new { message = "Book updated successfully." });
            }
            return BadRequest(new { message = "Failed to update book details." });
        }

        [HttpPost("{id}/status")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var userName = User.Identity?.Name ?? "api-admin";
            var success = await _bookBusiness.UpdateStatus(id, userName);
            if (success)
            {
                return Ok(new { message = "Book status updated successfully." });
            }
            return BadRequest(new { message = "Failed to update book status." });
        }
    }
}
