using LibrarySystem.Business.PublicationBusiness;
using LibrarySystem.Shared.PublicationData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PublicationController : ControllerBase
    {
        private readonly IPublicationBusiness _publicationBusiness;

        public PublicationController(IPublicationBusiness publicationBusiness)
        {
            _publicationBusiness = publicationBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var publications = await _publicationBusiness.GetList();
            return Ok(publications);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var publication = await _publicationBusiness.GetDetails(id);
            if (publication == null)
            {
                return NotFound(new { message = $"Publication with ID {id} not found." });
            }
            return Ok(publication);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] PublicationDetails publication)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            publication.User = User.Identity?.Name ?? "api-admin";
            var success = await _publicationBusiness.Add(publication);
            if (success)
            {
                return Ok(new { message = "Publication created successfully." });
            }
            return BadRequest(new { message = "Failed to create publication." });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] PublicationDetails publication)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            publication.PublicationId = id;
            publication.User = User.Identity?.Name ?? "api-admin";
            var success = await _publicationBusiness.Edit(publication);
            if (success)
            {
                return Ok(new { message = "Publication updated successfully." });
            }
            return BadRequest(new { message = "Failed to update publication details." });
        }

        [HttpPost("{id}/status")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var userName = User.Identity?.Name ?? "api-admin";
            var success = await _publicationBusiness.UpdateStatus(id, userName);
            if (success)
            {
                return Ok(new { message = "Publication status updated successfully." });
            }
            return BadRequest(new { message = "Failed to update publication status." });
        }
    }
}
