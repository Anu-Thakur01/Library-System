using LibrarySystem.Repository.Data;
using LibrarySystem.Repository.Models;
using LibrarySystem.Shared.ReportData;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Repository.ReportRepository
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDbContext _context;

        public ReportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BorrowedBookReportDetails>> GetBorrowedBooksByDateRange(DateTime fromDate, DateTime toDate)
        {
            var from = fromDate.Date;
            var to = toDate.Date.AddDays(1);

            var borrows = await _context.Borrows
                .AsNoTracking()
                .Include(borrow => borrow.Book)
                .Include(borrow => borrow.Member)
                .Where(borrow => borrow.BorrowedOn >= from && borrow.BorrowedOn < to)
                .OrderByDescending(borrow => borrow.BorrowedOn)
                .ThenBy(borrow => borrow.Member.MemberName)
                .ToListAsync();

            return borrows.Select(MapBorrow).ToList();
        }

        public async Task<List<MemberBorrowReportDetails>> GetMemberBorrowHistory(string memberName)
        {
            var searchText = memberName.Trim();

            var memberQuery = _context.Member
                .AsNoTracking()
                .Where(member => member.MemberName != null);

            memberQuery = _context.Database.IsNpgsql()
                ? memberQuery.Where(member => EF.Functions.ILike(member.MemberName!, $"%{searchText}%"))
                : memberQuery.Where(member => member.MemberName!.ToLower().Contains(searchText.ToLower()));

            var members = await memberQuery.OrderBy(member => member.MemberName).ToListAsync();

            var memberIds = members.Select(member => member.MemberId).ToList();
            var borrows = await _context.Borrows
                .AsNoTracking()
                .Include(borrow => borrow.Book)
                .Include(borrow => borrow.Member)
                .Where(borrow => memberIds.Contains(borrow.MemberId))
                .OrderByDescending(borrow => borrow.BorrowedOn)
                .ToListAsync();

            return members.Select(member =>
            {
                var memberBorrows = borrows
                    .Where(borrow => borrow.MemberId == member.MemberId)
                    .Select(MapBorrow)
                    .ToList();

                return new MemberBorrowReportDetails
                {
                    MemberId = member.MemberId,
                    MemberName = member.MemberName,
                    PhoneNumber = member.PhoneNumber,
                    Address = member.Address,
                    Email = member.Email,
                    JoinedDate = member.JoinedDate,
                    ExpirationDate = member.ExpirationDate,
                    MembershipType = member.MembershipType,
                    PresentBorrows = memberBorrows.Where(borrow => !borrow.IsReturned).ToList(),
                    BorrowHistory = memberBorrows.Where(borrow => borrow.IsReturned).ToList()
                };
            }).ToList();
        }

        public async Task<List<string>> GetMemberNameSuggestions(string searchText)
        {
            var term = searchText.Trim();

            var memberQuery = _context.Member
                .AsNoTracking()
                .Where(member => member.MemberName != null);

            memberQuery = _context.Database.IsNpgsql()
                ? memberQuery.Where(member => EF.Functions.ILike(member.MemberName!, $"%{term}%"))
                : memberQuery.Where(member => member.MemberName!.ToLower().Contains(term.ToLower()));

            return await memberQuery
                .OrderBy(member => member.MemberName)
                .Select(member => member.MemberName ?? string.Empty)
                .Where(memberName => !string.IsNullOrWhiteSpace(memberName))
                .Distinct()
                .Take(10)
                .ToListAsync();
        }

        private static BorrowedBookReportDetails MapBorrow(Borrow borrow)
        {
            return new BorrowedBookReportDetails
            {
                BorrowId = borrow.BorrowId,
                BorrowedOn = borrow.BorrowedOn,
                DueDate = borrow.DueDate,
                IsReturned = borrow.IsReturned,
                IsOverdue = borrow.IsOverdue,
                DaysOverdue = borrow.DaysOverdue,
                FineAmount = borrow.FineAmount,
                Status = borrow.Status,

                BookId = borrow.BookId,
                BookName = borrow.Book?.Name,
                Author = borrow.Book?.Author,
                Publication = borrow.Book?.Publication,
                Category = borrow.Book?.Category,
                Isbn = borrow.Book?.Isbn,
                Edition = borrow.Book?.Edition,

                MemberId = borrow.MemberId,
                MemberName = borrow.Member?.MemberName,
                PhoneNumber = borrow.Member?.PhoneNumber ?? 0,
                Address = borrow.Member?.Address,
                Email = borrow.Member?.Email,
                JoinedDate = borrow.Member?.JoinedDate ?? default,
                ExpirationDate = borrow.Member?.ExpirationDate ?? default,
                MembershipType = borrow.Member?.MembershipType
            };
        }
    }
}
