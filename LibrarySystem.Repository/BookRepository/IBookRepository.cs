using LibrarySystem.Repository.Models;
using LibrarySystem.Shared.BookData;

namespace LibrarySystem.Repository.BookRepository
{
    public interface IBookRepository
    {
        Task<bool> AddBook(Book book);
        Task<bool> EditBooks(BookDetails book);
        Task<Book> GetBookDetails(int id);
        Task<List<Book>> GetBookList();
        Task<bool> UpdateStatus(int bookId, string user);
        Task<bool> DeleteBook(int bookId);
    }
}
