using LibrarySystem.Shared.BookData;

namespace LibrarySystem.Business.BookBusiness
{
    public interface IBookBusiness
    {
        Task<bool> AddBook(BookDetails book);
        Task<bool> EditBooks(BookDetails book);
        Task<BookDetails> GetBookDetails(int id);
        Task<List<BookDetails>> GetBookList();
        Task<bool> UpdateStatus(int bookId, string user);
        Task<bool> DeleteBook(int bookId);
    }
}
