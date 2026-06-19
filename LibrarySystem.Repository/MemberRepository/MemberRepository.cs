using LibrarySystem.Repository.Data;
using LibrarySystem.Repository.Models;
using LibrarySystem.Shared.MemberData;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Repository.MemberRepository
{
    public class MemberRepository : IMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public MemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Add(Member data)
        {
            await _context.Member.AddAsync(data);
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

        public async Task<bool> Edit(MemberDetails member)
        {
            var details = _context.Member.FirstOrDefault(x => x.MemberId == member.MemberId);
            if (details != null)
            {
                details.MemberName = member.MemberName;
                details.PhoneNumber = member.PhoneNumber;
                details.Address = member.Address;
                details.Email = member.Email;
                details.JoinedDate = member.JoinedDate;
                details.MembershipType = member.MembershipType;
                details.Status = member.Status;
                details.ModifiedBy = member.User;
                details.ModifiedDate = DateTime.Now;
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return true;
            }
            return false;
        }

        public async Task<bool> Delete(int memberId)
        {
            var details = await _context.Member.FirstOrDefaultAsync(x => x.MemberId == memberId);
            if (details != null)
            {
                _context.Member.Remove(details);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return true;
            }
            return false;
        }

        public async Task<Member> GetDetails(int id)
        {
            var details = await _context.Member.AsNoTracking().FirstOrDefaultAsync(x => x.MemberId == id);
            return details;
        }

        public async Task<List<Member>> GetList()
        {
            var listValue = await _context.Member.AsNoTracking().OrderByDescending(x => x.MemberId).ToListAsync();
            return listValue;
        }

        public async Task<bool> UpdateStatus(int memberId, string user)
        {
            var details = await _context.Member.FirstOrDefaultAsync(x => x.MemberId == memberId);
            if (details != null)
            {
                details.Status = details.Status == "A" ? "N" : "A";
                details.ModifiedBy = user;
                details.ModifiedDate = DateTime.Now;
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return true;
            }
            return false;
        }
    }
}
