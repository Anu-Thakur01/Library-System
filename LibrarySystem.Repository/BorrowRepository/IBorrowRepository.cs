using LibrarySystem.Repository.Models;
using LibrarySystem.Shared.BorrowData;

namespace LibrarySystem.Repository.BorrowRepository
{
    public interface IBorrowRepository
    {
        Task<bool> AddBorrow(Borrow borrow);
        Task<bool> EditBorrow(BorrowDetails borrow);
        Task<Borrow> GetBorrowDetails(int id);
        Task<List<Borrow>> GetBorrowList();
        Task<bool> UpdateStatus(int borrowId, string user);
        Task<bool> ReturnBook(int borrowId, decimal fineAmountPerDay);
        Task<bool> IsMemberAlreadyBorrowedBook(int memberId, int bookId);
        Task<int> GetMemberActiveBorrowCount(int memberId);
    }
}
