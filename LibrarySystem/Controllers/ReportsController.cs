using LibrarySystem.Business.ReportBusiness;
using LibrarySystem.Shared.ReportData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "SuperAdmin,Staff")]
    public class ReportsController : Controller
    {
        private readonly IReportBusiness _reportBusiness;

        public ReportsController(IReportBusiness reportBusiness)
        {
            _reportBusiness = reportBusiness;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult BorrowedBooks()
        {
            return View(new BorrowedBooksReportSearch
            {
                FromDate = DateTime.Today.AddDays(-30),
                ToDate = DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrowedBooks(BorrowedBooksReportSearch search)
        {
            if (!search.FromDate.HasValue)
            {
                ModelState.AddModelError(nameof(search.FromDate), "From date is required.");
            }

            if (!search.ToDate.HasValue)
            {
                ModelState.AddModelError(nameof(search.ToDate), "To date is required.");
            }

            if (search.FromDate.HasValue && search.ToDate.HasValue && search.FromDate.Value.Date > search.ToDate.Value.Date)
            {
                ModelState.AddModelError(nameof(search.ToDate), "To date must be greater than or equal to from date.");
            }

            if (!ModelState.IsValid)
            {
                return View(search);
            }

            search.Results = await _reportBusiness.GetBorrowedBooksByDateRange(search.FromDate!.Value, search.ToDate!.Value);
            return View(search);
        }

        [HttpGet]
        public IActionResult MemberBorrowHistory()
        {
            return View(new MemberBorrowHistorySearch());
        }

        [HttpGet]
        public async Task<IActionResult> MemberNameSuggestions(string term)
        {
            var suggestions = await _reportBusiness.GetMemberNameSuggestions(term ?? string.Empty);
            return Json(suggestions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MemberBorrowHistory(MemberBorrowHistorySearch search)
        {
            if (string.IsNullOrWhiteSpace(search.MemberName))
            {
                ModelState.AddModelError(nameof(search.MemberName), "Member name is required.");
                return View(search);
            }

            search.Results = await _reportBusiness.GetMemberBorrowHistory(search.MemberName);
            return View(search);
        }
    }
}
