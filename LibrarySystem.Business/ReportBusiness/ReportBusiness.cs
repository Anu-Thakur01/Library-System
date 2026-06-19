using LibrarySystem.Repository.ReportRepository;
using LibrarySystem.Shared.ReportData;

namespace LibrarySystem.Business.ReportBusiness
{
    public class ReportBusiness : IReportBusiness
    {
        private readonly IReportRepository _reportRepository;

        public ReportBusiness(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<List<BorrowedBookReportDetails>> GetBorrowedBooksByDateRange(DateTime fromDate, DateTime toDate)
        {
            return await _reportRepository.GetBorrowedBooksByDateRange(fromDate, toDate);
        }

        public async Task<List<MemberBorrowReportDetails>> GetMemberBorrowHistory(string memberName)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                return new List<MemberBorrowReportDetails>();
            }

            return await _reportRepository.GetMemberBorrowHistory(memberName);
        }

        public async Task<List<string>> GetMemberNameSuggestions(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return new List<string>();
            }

            return await _reportRepository.GetMemberNameSuggestions(searchText);
        }
    }
}
