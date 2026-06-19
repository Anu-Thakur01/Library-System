using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Shared.UserData
{
    public class UserListDetails
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string CustomNotes { get; set; } = string.Empty;
    }

    public class UserCreateDetails
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least {2} characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare(nameof(Password), ErrorMessage = "Password and confirm password do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Custom Notes")]
        public string? CustomNotes { get; set; }
    }

    public class UserEditDetails
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be at least {2} characters long")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [Compare(nameof(NewPassword), ErrorMessage = "New password and confirm password do not match")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        public string? ConfirmNewPassword { get; set; }

        [Display(Name = "Custom Notes")]
        public string? CustomNotes { get; set; }
    }

    public class UserOperationResult
    {
        public bool Succeeded { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static UserOperationResult Success()
        {
            return new UserOperationResult { Succeeded = true };
        }

        public static UserOperationResult Failed(IEnumerable<string> errors)
        {
            return new UserOperationResult
            {
                Succeeded = false,
                Errors = errors.ToList()
            };
        }

        public static UserOperationResult Failed(string error)
        {
            return Failed(new[] { error });
        }
    }
}
