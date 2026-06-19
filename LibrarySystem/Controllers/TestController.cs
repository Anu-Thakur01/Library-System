using LibrarySystem.Models;
using LibrarySystem.Repository.Data;
using LibrarySystem.Repository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class TestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var book = new Book
            {
                Name = "The Silent Horizon",
                Author = "Emily Carter",
                Publication = "BlueWave Publishing",
                Category = "Science Fiction",
                Edition = "3rd Edition",
                Year = "2024",
                TotalCopies = 25,
                AvailableCopies = 18,
                PublishedYear = "2024",
                Isbn = "978-1-4028-9462-6",
                NoOfPages = 412,
                Status = "Available",
                CreatedDate = DateTime.Now,
                CreatedBy = "Admin",
                ModifiedDate = DateTime.Now,
                ModifiedBy = "Admin"
            };
            _context.Books.Add(book);
            _context.SaveChanges();
            return View();
        }

        public IActionResult PassValueFromController()
        {
            
            //var books = new List<Book>();
            //books.Add(new Book { 
            //    BookName = "The Great Gatsby", 
            //    BookAuthor = "F. Scott Fitzgerald", 
            //    BookType = "Novel", 
            //    BookYear = "1925" }
            //);

            


            var bookDetails = new List<Book>
            {
                new Book { Name = "To Kill a Mockingbird", Author = "Harper Lee", Category = "Novel", Year = "1960" },
                new Book { Name = "1984", Author = "George Orwell", Category = "Dystopian", Year = "1949" }
            };
            //return View(bookDetails);

            //ViewData
            var bookValue = new Book
            {
                Name = "The Great Gatsby",
                Author = "F. Scott Fitzgerald",
                Category = "Novel",
                Year = "1925"
            };
            ViewData["BookDetails"] = bookValue;

            //ViewBag
            ViewBag.BookDetails = bookValue;

            TempData.Keep();

            var bookList = _context.Books.ToList();
            return View(bookList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    
}
