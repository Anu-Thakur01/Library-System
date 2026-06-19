using LibrarySystem.Business.CategoryBusiness;
using LibrarySystem.Shared.CategoryData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryBusiness _categoryBusiness;

        public CategoryController(ICategoryBusiness categoryBusiness)
        {
            _categoryBusiness = categoryBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var categories = await _categoryBusiness.GetList();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var category = await _categoryBusiness.GetDetails(id);
            if (category == null)
            {
                return NotFound(new { message = $"Category with ID {id} not found." });
            }
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] CategoryDetails category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            category.User = User.Identity?.Name ?? "api-admin";
            var success = await _categoryBusiness.Add(category);
            if (success)
            {
                return Ok(new { message = "Category created successfully." });
            }
            return BadRequest(new { message = "Failed to create category." });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDetails category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            category.CategoryId = id;
            category.User = User.Identity?.Name ?? "api-admin";
            var success = await _categoryBusiness.Edit(category);
            if (success)
            {
                return Ok(new { message = "Category updated successfully." });
            }
            return BadRequest(new { message = "Failed to update category." });
        }

        [HttpPost("{id}/status")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var userName = User.Identity?.Name ?? "api-admin";
            var success = await _categoryBusiness.UpdateStatus(id, userName);
            if (success)
            {
                return Ok(new { message = "Category status updated successfully." });
            }
            return BadRequest(new { message = "Failed to update category status." });
        }
    }
}
