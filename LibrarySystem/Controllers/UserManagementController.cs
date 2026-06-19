using LibrarySystem.Business.UserBusiness;
using LibrarySystem.Shared.UserData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class UserManagementController : Controller
    {
        private readonly IUserBusiness _userBusiness;

        public UserManagementController(IUserBusiness userBusiness)
        {
            _userBusiness = userBusiness;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userBusiness.GetList();
            return View(users);
        }

        public async Task<IActionResult> Add()
        {
            await PopulateRoles();
            return View(new UserCreateDetails());
        }

        [HttpGet]
        public async Task<IActionResult> IsUsernameAvailable(string userName)
        {
            var isAvailable = await _userBusiness.IsUsernameAvailable(userName ?? string.Empty);
            return Json(new { isAvailable });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(UserCreateDetails userDetails)
        {
            if (!ModelState.IsValid)
            {
                await PopulateRoles(userDetails.Role);
                return View(userDetails);
            }

            if (!await _userBusiness.IsUsernameAvailable(userDetails.UserName))
            {
                ModelState.AddModelError(nameof(userDetails.UserName), "Username is already taken.");
                await PopulateRoles(userDetails.Role);
                return View(userDetails);
            }

            var result = await _userBusiness.Add(userDetails);
            if (result.Succeeded)
            {
                TempData["isSuccess"] = "YES";
                TempData["Message"] = "User added successfully";
                return RedirectToAction("Index");
            }

            AddErrors(result.Errors);
            await PopulateRoles(userDetails.Role);
            return View(userDetails);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Index");
            }

            var userDetails = await _userBusiness.GetDetails(id);
            if (userDetails == null)
            {
                return NotFound();
            }

            await PopulateRoles(userDetails.Role);
            return View(userDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditDetails userDetails)
        {
            ModelState.Remove(nameof(UserEditDetails.UserName));

            var existingUserDetails = await _userBusiness.GetDetails(userDetails.Id);
            if (existingUserDetails == null)
            {
                return NotFound();
            }

            userDetails.UserName = existingUserDetails.UserName;

            if (!ModelState.IsValid)
            {
                await PopulateRoles(userDetails.Role);
                return View(userDetails);
            }

            var result = await _userBusiness.Edit(userDetails);
            if (result.Succeeded)
            {
                TempData["isSuccess"] = "YES";
                TempData["Message"] = "User updated successfully";
                return RedirectToAction("Index");
            }

            AddErrors(result.Errors);
            await PopulateRoles(userDetails.Role);
            return View(userDetails);
        }

        private async Task PopulateRoles(string? selectedRole = null)
        {
            var roles = await _userBusiness.GetRoles();
            ViewBag.RoleList = roles.Select(role => new SelectListItem
            {
                Text = role,
                Value = role,
                Selected = role == selectedRole
            }).ToList();
        }

        private void AddErrors(IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}
