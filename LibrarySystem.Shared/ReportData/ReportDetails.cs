using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Shared.ReportData
{
    public class BorrowedBooksReportSearch
    {
        [DataType(DataType.Date)]
        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }

        public List<BorrowedBookReportDetails> Results { get; set; } = new List<BorrowedBookReportDetails>();
    }

    public class MemberBorrowHistorySearch
    {
        [Display(Name = "Member Name")]
        public string? MemberName { get; set; }

        public List<MemberBorrowReportDetails> Results { get; set; } = new List<MemberBorrowReportDetails>();
    }

    public class BorrowedBookReportDetails
    {
        public int BorrowId { get; set; }
        public DateTime BorrowedOn { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsReturned { get; set; }
        public bool IsOverdue { get; set; }
        public int DaysOverdue { get; set; }
        public decimal FineAmount { get; set; }
        public string? Status { get; set; }

        public int BookId { get; set; }
        public string? BookName { get; set; }
        public string? Author { get; set; }
        public string? Publication { get; set; }
        public string? Category { get; set; }
        public string? Isbn { get; set; }
        public string? Edition { get; set; }

        public int MemberId { get; set; }
        public string? MemberName { get; set; }
        public long PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public DateTime JoinedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string? MembershipType { get; set; }
    }

    public class MemberBorrowReportDetails
    {
        public int MemberId { get; set; }
        public string? MemberName { get; set; }
        public long PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public DateTime JoinedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string? MembershipType { get; set; }
        public List<BorrowedBookReportDetails> PresentBorrows { get; set; } = new List<BorrowedBookReportDetails>();
        public List<BorrowedBookReportDetails> BorrowHistory { get; set; } = new List<BorrowedBookReportDetails>();
    }
}
