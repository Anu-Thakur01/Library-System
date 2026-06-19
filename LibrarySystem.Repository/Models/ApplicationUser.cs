using Microsoft.AspNetCore.Identity;

namespace LibrarySystem.Repository.Models
{
    /// <summary>
    /// Custom IdentityUser model extending the default database schema
    /// with administrative metadata columns for our users.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public string CustomNotes { get; set; } = string.Empty;
    }
}
