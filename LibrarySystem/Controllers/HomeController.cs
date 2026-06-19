using LibrarySystem.Business.AuthorBusiness;
using LibrarySystem.Business.BookBusiness;
using LibrarySystem.Business.BorrowBusiness;
using LibrarySystem.Business.CategoryBusiness;
using LibrarySystem.Business.MemberBusiness;
using LibrarySystem.Business.PublicationBusiness;
using LibrarySystem.Dtos;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LibrarySystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IBookBusiness _bookBusiness;
        private readonly IAuthorBusiness _authorBusiness;
        private readonly IMemberBusiness _memberBusiness;
        private readonly IBorrowBusiness _borrowBusiness;
        private readonly ICategoryBusiness _categoryBusiness;
        private readonly IPublicationBusiness _publicationBusiness;

        public HomeController(
            IBookBusiness bookBusiness,
            IAuthorBusiness authorBusiness,
            IMemberBusiness memberBusiness,
            IBorrowBusiness borrowBusiness,
            ICategoryBusiness categoryBusiness,
            IPublicationBusiness publicationBusiness)
        {
            _bookBusiness = bookBusiness;
            _authorBusiness = authorBusiness;
            _memberBusiness = memberBusiness;
            _borrowBusiness = borrowBusiness;
            _categoryBusiness = categoryBusiness;
            _publicationBusiness = publicationBusiness;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookBusiness.GetBookList();
            var authors = await _authorBusiness.GetList();
            var members = await _memberBusiness.GetList();
            var borrows = await _borrowBusiness.GetBorrowList();
            var categories = await _categoryBusiness.GetList();
            var publications = await _publicationBusiness.GetList();

            var model = new DashboardViewModel
            {
                TotalBooks = books.Count,
                ActiveBooks = books.Count(x => x.Status == "A"),
                TotalAuthors = authors.Count,
                TotalMembers = members.Count,
                ActiveBorrows = borrows.Count(x => !x.IsReturned),
                OverdueBorrows = borrows.Count(x => !x.IsReturned && x.DueDate.Date < DateTime.Today),
                Categories = categories.Count,
                Publications = publications.Count,
                RecentBooks = books.Take(5).ToList(),
                RecentBorrows = borrows.Take(5).ToList()
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    
}
