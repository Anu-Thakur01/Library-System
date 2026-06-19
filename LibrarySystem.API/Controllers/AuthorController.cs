using LibrarySystem.Business.AuthorBusiness;
using LibrarySystem.Shared.AuthorData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorBusiness _authorBusiness;

        public AuthorController(IAuthorBusiness authorBusiness)
        {
            _authorBusiness = authorBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var authors = await _authorBusiness.GetList();
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var author = await _authorBusiness.GetDetails(id);
            if (author == null)
            {
                return NotFound(new { message = $"Author with ID {id} not found." });
            }
            return Ok(author);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] AuthorDetails author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            author.User = User.Identity?.Name ?? "api-admin";
            var success = await _authorBusiness.Add(author);
            if (success)
            {
                return Ok(new { message = "Author created successfully." });
            }
            return BadRequest(new { message = "Failed to create author." });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorDetails author)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            author.AuthorId = id;
            author.User = User.Identity?.Name ?? "api-admin";
            var success = await _authorBusiness.Edit(author);
            if (success)
            {
                return Ok(new { message = "Author updated successfully." });
            }
            return BadRequest(new { message = "Failed to update author details." });
        }

        [HttpPost("{id}/status")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var userName = User.Identity?.Name ?? "api-admin";
            var success = await _authorBusiness.UpdateStatus(id, userName);
            if (success)
            {
                return Ok(new { message = "Author status updated successfully." });
            }
            return BadRequest(new { message = "Failed to update author status." });
        }
    }
}
