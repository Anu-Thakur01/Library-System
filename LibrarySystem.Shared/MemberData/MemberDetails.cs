using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Shared.MemberData;

public class MemberDetails
{
    public int MemberId { get; set; }
    [Required(ErrorMessage = "Member name is required.")]
    public string? MemberName { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [Range(1000000000, 9999999999, ErrorMessage = "Phone number must be exactly 10 digits.")]
    public long PhoneNumber { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Email address must be valid.")]
    [RegularExpression(@"^[A-Za-z0-9._%+-]+@[Gg][Mm][Aa][Ii][Ll]\.[Cc][Oo][Mm]$", ErrorMessage = "Email address must be a valid Gmail address.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Joined date is required.")]
    public DateTime JoinedDate { get; set; }
    public DateTime ExpirationDate { get; set; }

    [Required(ErrorMessage = "Membership type is required.")]
    public string? MembershipType { get; set; }
    public string? Status { get; set; }
    public string? User { get; set; }

    public MemberDetails()
    {
        this.ExpirationDate = JoinedDate.AddDays(15);
    }
}
