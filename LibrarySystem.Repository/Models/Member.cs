using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Repository.Models;

public class Member : BaseEntity
{
    [Key]
    public int MemberId { get; set; }
    [Required]
    public string? MemberName { get; set; }
    [Required]
    public long PhoneNumber { get; set; }
    [Required]
    public string? Address { get; set; }
    [Required]
    public string? Email { get; set; }
    [Required]
    public DateTime JoinedDate { get; set; }
    [Required]
    public DateTime ExpirationDate { get; set; }
    [Required]
    public string? MembershipType { get; set; }

}