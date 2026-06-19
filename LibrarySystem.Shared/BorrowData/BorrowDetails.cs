using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibrarySystem.Shared.BorrowData
{
    public class BorrowDetails
    {
        public int BorrowId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Required]
        public DateTime BorrowedOn { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        public bool IsOverdue { get; set; } = false;

        public bool IsReturned { get; set; } = false;

        public int DaysOverdue { get; set; } = 0;

        public decimal FineAmount { get; set; } = 0;

        public string? BookName { get; set; }

        public string? MemberName { get; set; }

        public string? Status { get; set; }

        public string? User { get; set; }

        public List<SelectListItem> BookList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> MemberList { get; set; } = new List<SelectListItem>();
    }
}
