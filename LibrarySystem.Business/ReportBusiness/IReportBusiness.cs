using LibrarySystem.Shared.ReportData;

namespace LibrarySystem.Business.ReportBusiness
{
    public interface IReportBusiness
    {
        Task<List<BorrowedBookReportDetails>> GetBorrowedBooksByDateRange(DateTime fromDate, DateTime toDate);
        Task<List<MemberBorrowReportDetails>> GetMemberBorrowHistory(string memberName);
        Task<List<string>> GetMemberNameSuggestions(string searchText);
    }
}
