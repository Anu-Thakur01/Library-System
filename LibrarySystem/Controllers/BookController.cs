using LibrarySystem.Business.AuthorBusiness;
using LibrarySystem.Business.BookBusiness;
using LibrarySystem.Business.CategoryBusiness;
using LibrarySystem.Business.PublicationBusiness;
using LibrarySystem.Shared.BookData;
using LibrarySystem.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibrarySystem.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class BookController : Controller
    {
        private readonly IBookBusiness _bookBusiness;
        private readonly IAuthorBusiness _authorBusiness;
        private readonly ICategoryBusiness _categoryBusiness;
        private readonly IPublicationBusiness _publicationBusiness;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BookController(IBookBusiness bookBusiness, IAuthorBusiness authorBusiness, ICategoryBusiness categoryBusiness, IPublicationBusiness publicationBusiness, IWebHostEnvironment webHostEnvironment)
        {
            _bookBusiness = bookBusiness;
            _authorBusiness = authorBusiness;
            _categoryBusiness = categoryBusiness;
            _publicationBusiness = publicationBusiness;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var bookList = await _bookBusiness.GetBookList();
            return View(bookList);
        }

        public async Task<IActionResult> AddBook()
        {
            BookDetails bookDetails = new BookDetails();
            await PopulateDropdowns(bookDetails);
            return View(bookDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBook(BookDetails book)
        {
            if (book.ImageFile != null)
            {
                var extension = Path.GetExtension(book.ImageFile.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImageFile", "Only .jpg, .jpeg, and .png images are allowed.");
                }
            }

            if (ModelState.IsValid)
            {
                if (book.ImageFile != null)
                {
                    book.ImageUrl = await BookImageHelper.UploadImageAsync(book.ImageFile, _webHostEnvironment.WebRootPath);
                }

                book.User = "admin";
                bool isAdded = await _bookBusiness.AddBook(book);
                if (isAdded)
                {
                    TempData["isSuccess"] = "YES";
                    TempData["Message"] = "Book added successfully";
                }
                else
                {
                    TempData["isSuccess"] = "YES";
                    TempData["Message"] = "Failed to add book";
                }
                return RedirectToAction("Index");
            }
            else
            {
                // repopulate dropdowns before returning the view
                await PopulateDropdowns(book);
                return View(book);
            }
        }


        public async Task<IActionResult> EditBook(string id)
        {
            var bookId = Convert.ToInt32(id);
            var bookDetails = await _bookBusiness.GetBookDetails(bookId);
            
            await PopulateDropdowns(bookDetails);
            return View(bookDetails);
        }

        public async Task<IActionResult> UpdateStatus(string id)
        {
            var bookId = Convert.ToInt32(id);
            var user = "admin";
            var isUpdated = await _bookBusiness.UpdateStatus(bookId,user);
            if (isUpdated)
            {
                TempData["isSuccess"] = "YES";
                TempData["Message"] = "Book status updated successfully";
            }
            else
            {
                TempData["isSuccess"] = "YES";
                TempData["Message"] = "Failed to update book status";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var isDeleted = await _bookBusiness.DeleteBook(id);
            if (isDeleted)
            {
                TempData["isSuccess"] = "YES";
                TempData["Message"] = "Book deleted successfully";
            }
            else
            {
                TempData["isSuccess"] = "NO";
                TempData["Message"] = "Failed to delete book";
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBook(BookDetails book)
        {
            if (book.ImageFile != null)
            {
                var extension = Path.GetExtension(book.ImageFile.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImageFile", "Only .jpg, .jpeg, and .png images are allowed.");
                }
            }

            if (ModelState.IsValid)
            {
                if (book.ImageFile != null)
                {
                    book.ImageUrl = await BookImageHelper.UploadImageAsync(book.ImageFile, _webHostEnvironment.WebRootPath);
                }

                book.User = "admin";
                var details = await _bookBusiness.EditBooks(book);
                if (details)
                {
                    TempData["isSuccess"] = "YES";
                    TempData["Message"] = "Book details updated successfully";
                }
                else
                {
                    TempData["isSuccess"] = "NO";
                    TempData["Message"] = "Failed to update book details";
                }
                return RedirectToAction("Index");
            }
            else
            {
                // repopulate dropdowns before returning the view
                await PopulateDropdowns(book);
                return View(book);
            }
        }

        [HttpGet]
        public IActionResult ViewSecureImage(string fileName, string signature)
        {
            if (!BookImageHelper.VerifySignature(fileName, signature))
            {
                return Forbid(); // Return 403 Forbidden on invalid or missing proof
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Double check existence and path traversal safety
            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists || !fileInfo.DirectoryName!.Equals(uploadsFolder, StringComparison.OrdinalIgnoreCase))
            {
                return NotFound("Image not found.");
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            return PhysicalFile(filePath, contentType);
        }

        private async Task PopulateDropdowns(BookDetails book)
        {
            var authorList = await _authorBusiness.GetList();
            book.AuthorList = authorList.Select(a => new SelectListItem
            {
                Value = a.AuthorId.ToString(),
                Text = a.FirstName + " " + a.LastName,
                Selected = a.AuthorId.ToString() == book.Author
            }).ToList();

            var categoryList = await _categoryBusiness.GetList();
            book.CategoryList = categoryList.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name,
                Selected = c.CategoryId.ToString() == book.Category
            }).ToList();

            var publicationList = await _publicationBusiness.GetList();
            book.PublicationList = publicationList.Select(p => new SelectListItem
            {
                Value = p.PublicationId.ToString(),
                Text = p.PublicationName,
                Selected = p.PublicationId.ToString() == book.Publication
            }).ToList();
        }
    }
}


