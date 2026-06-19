using LibrarySystem.Shared.ReportData;

namespace LibrarySystem.Repository.ReportRepository
{
    public interface IReportRepository
    {
        Task<List<BorrowedBookReportDetails>> GetBorrowedBooksByDateRange(DateTime fromDate, DateTime toDate);
        Task<List<MemberBorrowReportDetails>> GetMemberBorrowHistory(string memberName);
        Task<List<string>> GetMemberNameSuggestions(string searchText);
    }
}
