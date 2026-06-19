using LibrarySystem.Repository.Data;
using LibrarySystem.Repository.Models;
using LibrarySystem.Shared.BookData;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Repository.BookRepository
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddBook(Book book)
        {
            await _context.Books.AddAsync(book);
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                var stackTrace = ex.StackTrace;
                return false;
            }
            
        }

        public async Task<bool> EditBooks(BookDetails book)
        {
            var bookDetails = _context.Books.FirstOrDefault(x=>x.BookId == book.BookId);
            if (bookDetails != null)
            {
                bookDetails.Name = book.Name;
                bookDetails.Author = book.Author;
                bookDetails.Publication = book.Publication;
                bookDetails.Category = book.Category;
                bookDetails.Isbn = book.Isbn;
                bookDetails.TotalCopies = book.TotalCopies;
                bookDetails.AvailableCopies = book.AvailableCopies;
                bookDetails.Edition = book.Edition;
                bookDetails.ModifiedBy = book.User;
                bookDetails.ModifiedDate = DateTime.Now;
                if (book.ImageUrl != null)
                {
                    bookDetails.ImageUrl = book.ImageUrl;
                }
                var result = await _context.SaveChangesAsync();
                if(result > 0)
                    return true;
            }
            return false;
        }
            
        public async Task<Book> GetBookDetails(int id)
        {
            var bookDetails = await _context.Books.AsNoTracking().FirstOrDefaultAsync(x => x.BookId == id);
            return bookDetails;
        }

        public async Task<List<Book>> GetBookList()
        {
            var bookList = await _context.Books.AsNoTracking().OrderByDescending(x=>x.BookId).ToListAsync();
            return bookList;
        }

        public async Task<bool> UpdateStatus(int bookId, string user)
        {
            var bookDetails = await _context.Books.FirstOrDefaultAsync(x => x.BookId == bookId);
            if (bookDetails != null)
            {
                bookDetails.Status = bookDetails.Status == "A" ? "N" : "A";
                bookDetails.ModifiedBy = user;
                bookDetails.ModifiedDate = DateTime.Now;
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return true;
            }
            return false;
        }

        public async Task<bool> DeleteBook(int bookId)
        {
            var bookDetails = await _context.Books.FirstOrDefaultAsync(x => x.BookId == bookId);
            if (bookDetails != null)
            {
                _context.Books.Remove(bookDetails);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return true;
            }
            return false;
        }
    }
}
