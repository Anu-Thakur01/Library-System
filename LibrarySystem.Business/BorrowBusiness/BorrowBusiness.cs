using LibrarySystem.Repository.BorrowRepository;
using LibrarySystem.Repository.Models;
using LibrarySystem.Shared.BorrowData;

namespace LibrarySystem.Business.BorrowBusiness
{
    public class BorrowBusiness : IBorrowBusiness
    {
        private readonly IBorrowRepository _borrowRepository;

        public BorrowBusiness(IBorrowRepository borrowRepository)
        {
            _borrowRepository = borrowRepository;
        }

        public async Task<bool> AddBorrow(BorrowDetails borrow)
        {
            var borrowEntity = new Borrow
            {
                BookId = borrow.BookId,
                MemberId = borrow.MemberId,
                BorrowedOn = borrow.BorrowedOn,
                DueDate = borrow.DueDate,
                IsOverdue = borrow.IsOverdue,
                IsReturned = borrow.IsReturned,
                DaysOverdue = borrow.DaysOverdue,
                FineAmount = borrow.FineAmount,
                CreatedBy = borrow.User,
                Status = borrow.Status
            };
            return await _borrowRepository.AddBorrow(borrowEntity);
        }

        public async Task<bool> EditBorrow(BorrowDetails borrow)
        {
            return await _borrowRepository.EditBorrow(borrow);
        }

        public async Task<BorrowDetails> GetBorrowDetails(int id)
        {
            var borrowData = await _borrowRepository.GetBorrowDetails(id);
            if (borrowData == null)
                return new BorrowDetails();

            var borrowDetails = new BorrowDetails
            {
                BorrowId = borrowData.BorrowId,
                BookId = borrowData.BookId,
                MemberId = borrowData.MemberId,
                BorrowedOn = borrowData.BorrowedOn,
                DueDate = borrowData.DueDate,
                IsOverdue = borrowData.IsOverdue,
                IsReturned = borrowData.IsReturned,
                DaysOverdue = borrowData.DaysOverdue,
                FineAmount = borrowData.FineAmount,
                BookName = borrowData.Book?.Name,
                MemberName = borrowData.Member?.MemberName,
                Status = borrowData.Status
            };
            return borrowDetails;
        }

        public async Task<List<BorrowDetails>> GetBorrowList()
        {
            var borrowList = await _borrowRepository.GetBorrowList();
            var borrowDetailsList = borrowList.Select(b => new BorrowDetails
            {
                BorrowId = b.BorrowId,
                BookId = b.BookId,
                MemberId = b.MemberId,
                BorrowedOn = b.BorrowedOn,
                DueDate = b.DueDate,
                IsOverdue = b.IsOverdue,
                IsReturned = b.IsReturned,
                DaysOverdue = b.DaysOverdue,
                FineAmount = b.FineAmount,
                BookName = b.Book?.Name,
                MemberName = b.Member?.MemberName,
                Status = b.Status
            }).ToList();
            return borrowDetailsList;
        }

        public async Task<bool> UpdateStatus(int borrowId, string user)
        {
            return await _borrowRepository.UpdateStatus(borrowId, user);
        }

        public async Task<bool> ReturnBook(int borrowId, decimal fineAmountPerDay)
        {
            return await _borrowRepository.ReturnBook(borrowId, fineAmountPerDay);
        }

        public async Task<(bool isValid, string errorMessage)> ValidateBorrow(int memberId, int bookId)
        {
            // Check if member already has this book and hasn't returned it
            var alreadyBorrowed = await _borrowRepository.IsMemberAlreadyBorrowedBook(memberId, bookId);
            if (alreadyBorrowed)
            {
                return (false, "This member has already borrowed this book and has not returned it yet.");
            }

            // Check if member has 3 or more active borrows
            var activeBorrowCount = await _borrowRepository.GetMemberActiveBorrowCount(memberId);
            if (activeBorrowCount >= 3)
            {
                return (false, "Member cannot borrow more than 3 books at once. Please return a book first.");
            }

            return (true, "");
        }
    }
}
