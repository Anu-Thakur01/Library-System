using LibrarySystem.Shared.BorrowData;

namespace LibrarySystem.Business.BorrowBusiness
{
    public interface IBorrowBusiness
    {
        Task<bool> AddBorrow(BorrowDetails borrow);
        Task<bool> EditBorrow(BorrowDetails borrow);
        Task<BorrowDetails> GetBorrowDetails(int id);
        Task<List<BorrowDetails>> GetBorrowList();
        Task<bool> UpdateStatus(int borrowId, string user);
        Task<bool> ReturnBook(int borrowId, decimal fineAmountPerDay);
        Task<(bool isValid, string errorMessage)> ValidateBorrow(int memberId, int bookId);
    }
}
