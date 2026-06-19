using LibrarySystem.Business.BookBusiness;
using LibrarySystem.Business.BorrowBusiness;
using LibrarySystem.Business.MemberBusiness;
using LibrarySystem.Shared.BorrowData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "SuperAdmin,Staff")]
    public class BorrowController : Controller
    {
        private readonly IBorrowBusiness _borrowBusiness;
        private readonly IBookBusiness _bookBusiness;
        private readonly IMemberBusiness _memberBusiness;
        private readonly IConfiguration _configuration;

        public BorrowController(IBorrowBusiness borrowBusiness, IBookBusiness bookBusiness, IMemberBusiness memberBusiness, IConfiguration configuration)
        {
            _borrowBusiness = borrowBusiness;
            _bookBusiness = bookBusiness;
            _memberBusiness = memberBusiness;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var borrowList = await _borrowBusiness.GetBorrowList();
            return View(borrowList);
        }

        public async Task<IActionResult> Add()
        {
            var bookList = await _bookBusiness.GetBookList();
            var memberList = await _memberBusiness.GetList();

            var borrow = new BorrowDetails
            {
                BookList = bookList.Select(b => new SelectListItem
                {
                    Text = b.Name,
                    Value = b.BookId.ToString()
                }).ToList(),
                MemberList = memberList.Select(m => new SelectListItem
                {
                    Text = m.MemberName,
                    Value = m.MemberId.ToString()
                }).ToList(),
                BorrowedOn = DateTime.Now
            };
            return View(borrow);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(BorrowDetails borrow)
        {
            if (ModelState.IsValid)
            {
                // Validate borrow rules
                var (isValid, errorMessage) = await _borrowBusiness.ValidateBorrow(borrow.MemberId, borrow.BookId);
                if (!isValid)
                {
                    ModelState.AddModelError("", errorMessage);
                    // Reload lists on error
                    var bookList = await _bookBusiness.GetBookList();
                    var memberList = await _memberBusiness.GetList();
                    borrow.BookList = bookList.Select(b => new SelectListItem
                    {
                        Text = b.Name,
                        Value = b.BookId.ToString()
                    }).ToList();
                    borrow.MemberList = memberList.Select(m => new SelectListItem
                    {
                        Text = m.MemberName,
                        Value = m.MemberId.ToString()
                    }).ToList();
                    return View(borrow);
                }

                borrow.User = User.Identity?.Name;
                borrow.Status = "Active";
                borrow.BorrowedOn = DateTime.Now;
                // Calculate due date based on configuration
                int dueDateDays = _configuration.GetValue<int>("LibrarySettings:DueDateDays", 15);
                borrow.DueDate = borrow.BorrowedOn.AddDays(dueDateDays);
                var result = await _borrowBusiness.AddBorrow(borrow);
                if (result)
                    return RedirectToAction("Index");
            }
            // Reload lists on error
            var bookListError = await _bookBusiness.GetBookList();
            var memberListError = await _memberBusiness.GetList();
            borrow.BookList = bookListError.Select(b => new SelectListItem
            {
                Text = b.Name,
                Value = b.BookId.ToString()
            }).ToList();
            borrow.MemberList = memberListError.Select(m => new SelectListItem
            {
                Text = m.MemberName,
                Value = m.MemberId.ToString()
            }).ToList();
            return View(borrow);
        }


        public async Task<IActionResult> Details(int id)
        {
            var borrow = await _borrowBusiness.GetBorrowDetails(id);
            return View(borrow);
        }

        [HttpPost]
        public async Task<IActionResult> ReturnBook(int id)
        {
            decimal fineAmountPerDay = _configuration.GetValue<decimal>("LibrarySettings:FineAmountPerDay", 5);
            var result = await _borrowBusiness.ReturnBook(id, fineAmountPerDay);
            if (result)
                return RedirectToAction("Index");
            return BadRequest();
        }
    }
}
