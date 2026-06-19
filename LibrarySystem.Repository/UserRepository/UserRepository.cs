using LibrarySystem.Repository.Models;
using LibrarySystem.Shared.UserData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Repository.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<UserOperationResult> Add(UserCreateDetails userDetails)
        {
            userDetails.UserName = userDetails.UserName?.Trim() ?? string.Empty;

            if (!await IsUsernameAvailable(userDetails.UserName))
            {
                return UserOperationResult.Failed("Username is already taken.");
            }

            if (!await _roleManager.RoleExistsAsync(userDetails.Role))
            {
                return UserOperationResult.Failed("Selected role does not exist.");
            }

            var user = new ApplicationUser
            {
                UserName = userDetails.UserName,
                Email = userDetails.Email,
                EmailConfirmed = true,
                FullName = userDetails.FullName,
                CustomNotes = userDetails.CustomNotes ?? string.Empty
            };

            var createResult = await _userManager.CreateAsync(user, userDetails.Password);
            if (!createResult.Succeeded)
            {
                return FromIdentityResult(createResult);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, userDetails.Role);
            if (roleResult.Succeeded)
            {
                return UserOperationResult.Success();
            }

            await _userManager.DeleteAsync(user);
            return FromIdentityResult(roleResult);
        }

        public async Task<UserOperationResult> Edit(UserEditDetails userDetails)
        {
            var user = await _userManager.FindByIdAsync(userDetails.Id);
            if (user == null)
            {
                return UserOperationResult.Failed("User not found.");
            }

            if (!await _roleManager.RoleExistsAsync(userDetails.Role))
            {
                return UserOperationResult.Failed("Selected role does not exist.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Contains("SuperAdmin") &&
                userDetails.Role != "SuperAdmin" &&
                await IsLastSuperAdmin(user))
            {
                return UserOperationResult.Failed("At least one SuperAdmin user must remain.");
            }

            user.FullName = userDetails.FullName;
            user.Email = userDetails.Email;
            user.CustomNotes = userDetails.CustomNotes ?? string.Empty;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return FromIdentityResult(updateResult);
            }

            if (currentRoles.Any())
            {
                var removeRoleResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRoleResult.Succeeded)
                {
                    return FromIdentityResult(removeRoleResult);
                }
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, userDetails.Role);
            if (!addRoleResult.Succeeded)
            {
                return FromIdentityResult(addRoleResult);
            }

            if (!string.IsNullOrWhiteSpace(userDetails.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, userDetails.NewPassword);
                if (!passwordResult.Succeeded)
                {
                    return FromIdentityResult(passwordResult);
                }
            }

            return UserOperationResult.Success();
        }

        public async Task<UserEditDetails?> GetDetails(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            return new UserEditDetails
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty,
                CustomNotes = user.CustomNotes
            };
        }

        public async Task<List<UserListDetails>> GetList()
        {
            var users = await _userManager.Users
                .AsNoTracking()
                .OrderBy(user => user.UserName)
                .ToListAsync();

            var userList = new List<UserListDetails>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserListDetails
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    Role = string.Join(", ", roles),
                    CustomNotes = user.CustomNotes
                });
            }

            return userList;
        }

        public async Task<List<string>> GetRoles()
        {
            return await _roleManager.Roles
                .AsNoTracking()
                .OrderBy(role => role.Name)
                .Select(role => role.Name ?? string.Empty)
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .ToListAsync();
        }

        public async Task<bool> IsUsernameAvailable(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            var existingUser = await _userManager.FindByNameAsync(userName.Trim());
            return existingUser == null;
        }

        private async Task<bool> IsLastSuperAdmin(ApplicationUser user)
        {
            var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
            return superAdmins.Count == 1 && superAdmins.Any(superAdmin => superAdmin.Id == user.Id);
        }

        private static UserOperationResult FromIdentityResult(IdentityResult result)
        {
            return UserOperationResult.Failed(result.Errors.Select(error => error.Description));
        }
    }
}
