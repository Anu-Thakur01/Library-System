using LibrarySystem.Repository.Data;
using LibrarySystem.Repository.Models;
using LibrarySystem.Shared.BorrowData;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Repository.BorrowRepository
{
    public class BorrowRepository : IBorrowRepository
    {
        private readonly ApplicationDbContext _context;

        public BorrowRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddBorrow(Borrow borrow)
        {
            await _context.Borrows.AddAsync(borrow);
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

        public async Task<bool> EditBorrow(BorrowDetails borrow)
        {
            var borrowDetails = _context.Borrows.FirstOrDefault(x => x.BorrowId == borrow.BorrowId);
            if (borrowDetails != null)
            {
                borrowDetails.BookId = borrow.BookId;
                borrowDetails.MemberId = borrow.MemberId;
                borrowDetails.BorrowedOn = borrow.BorrowedOn;
                borrowDetails.DueDate = borrow.DueDate;
                borrowDetails.IsOverdue = borrow.IsOverdue;
                borrowDetails.IsReturned = borrow.IsReturned;
                borrowDetails.DaysOverdue = borrow.DaysOverdue;
                borrowDetails.ModifiedBy = borrow.User;
                borrowDetails.ModifiedDate = DateTime.Now;
                borrowDetails.Status = borrow.Status;

                _context.Borrows.Update(borrowDetails);
                try
                {
                    var result = await _context.SaveChangesAsync();
                    return result > 0;
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    var stackTrace = ex.StackTrace;
                    return false;
                }
            }
            return false;
        }

        public async Task<Borrow> GetBorrowDetails(int id)
        {
            var result = await _context.Borrows
                .Include(b => b.Book)
                .Include(b => b.Member)
                .FirstOrDefaultAsync(x => x.BorrowId == id);
            return result ?? new Borrow();
        }

        public async Task<List<Borrow>> GetBorrowList()
        {
            return await _context.Borrows
                .Include(b => b.Book)
                .Include(b => b.Member)
                .ToListAsync();
        }

        public async Task<bool> UpdateStatus(int borrowId, string user)
        {
            var borrow = _context.Borrows.FirstOrDefault(x => x.BorrowId == borrowId);
            if (borrow != null)
            {
                borrow.ModifiedBy = user;
                borrow.ModifiedDate = DateTime.Now;
                _context.Borrows.Update(borrow);
                try
                {
                    var result = await _context.SaveChangesAsync();
                    return result > 0;
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    var stackTrace = ex.StackTrace;
                    return false;
                }
            }
            return false;
        }

        public async Task<bool> ReturnBook(int borrowId, decimal fineAmountPerDay)
        {
            var borrow = _context.Borrows.FirstOrDefault(x => x.BorrowId == borrowId);
            if (borrow != null)
            {
                borrow.IsReturned = true;
                
                var today = DateTime.Now.Date;
                var dueDate = borrow.DueDate.Date;
                if (today > dueDate)
                {
                    var lateDays = (today - dueDate).Days;
                    borrow.IsOverdue = true;
                    borrow.DaysOverdue = lateDays;
                    borrow.FineAmount = lateDays * fineAmountPerDay;
                }
                else
                {
                    borrow.IsOverdue = false;
                    borrow.DaysOverdue = 0;
                    borrow.FineAmount = 0;
                }

                borrow.ModifiedDate = DateTime.Now;
                _context.Borrows.Update(borrow);
                try
                {
                    var result = await _context.SaveChangesAsync();
                    return result > 0;
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    var stackTrace = ex.StackTrace;
                    return false;
                }
            }
            return false;
        }

        public async Task<bool> IsMemberAlreadyBorrowedBook(int memberId, int bookId)
        {
            var existingBorrow = await _context.Borrows
                .FirstOrDefaultAsync(x => x.MemberId == memberId && 
                                         x.BookId == bookId && 
                                         !x.IsReturned);
            return existingBorrow != null;
        }

        public async Task<int> GetMemberActiveBorrowCount(int memberId)
        {
            return await _context.Borrows
                .Where(x => x.MemberId == memberId && !x.IsReturned)
                .CountAsync();
        }
    }
}
