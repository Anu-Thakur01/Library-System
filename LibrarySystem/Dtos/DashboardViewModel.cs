using LibrarySystem.Shared.BookData;
using LibrarySystem.Shared.BorrowData;

namespace LibrarySystem.Dtos;

public class DashboardViewModel
{
    public int TotalBooks { get; set; }
    public int ActiveBooks { get; set; }
    public int TotalAuthors { get; set; }
    public int TotalMembers { get; set; }
    public int ActiveBorrows { get; set; }
    public int OverdueBorrows { get; set; }
    public int Categories { get; set; }
    public int Publications { get; set; }
    public List<BookDetails> RecentBooks { get; set; } = new();
    public List<BorrowDetails> RecentBorrows { get; set; } = new();
}
