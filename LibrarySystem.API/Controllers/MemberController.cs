using LibrarySystem.Business.MemberBusiness;
using LibrarySystem.Shared.MemberData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MemberController : ControllerBase
    {
        private readonly IMemberBusiness _memberBusiness;

        public MemberController(IMemberBusiness memberBusiness)
        {
            _memberBusiness = memberBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var members = await _memberBusiness.GetList();
            return Ok(members);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var member = await _memberBusiness.GetDetails(id);
            if (member == null)
            {
                return NotFound(new { message = $"Member with ID {id} not found." });
            }
            return Ok(member);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create([FromBody] MemberDetails member)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            member.User = User.Identity?.Name ?? "api-admin";
            var success = await _memberBusiness.Add(member);
            if (success)
            {
                return Ok(new { message = "Member created successfully." });
            }
            return BadRequest(new { message = "Failed to create member." });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Update(int id, [FromBody] MemberDetails member)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            member.MemberId = id;
            member.User = User.Identity?.Name ?? "api-admin";
            var success = await _memberBusiness.Edit(member);
            if (success)
            {
                return Ok(new { message = "Member details updated successfully." });
            }
            return BadRequest(new { message = "Failed to update member details." });
        }

        [HttpPost("{id}/status")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var userName = User.Identity?.Name ?? "api-admin";
            var success = await _memberBusiness.UpdateStatus(id, userName);
            if (success)
            {
                return Ok(new { message = "Member status updated successfully." });
            }
            return BadRequest(new { message = "Failed to update member status." });
        }
    }
}
