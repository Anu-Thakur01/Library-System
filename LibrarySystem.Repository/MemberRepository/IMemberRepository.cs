using LibrarySystem.Repository.Models;
using LibrarySystem.Shared.MemberData;

namespace LibrarySystem.Repository.MemberRepository
{
    public interface IMemberRepository
    {
        Task<bool> Add(Member memberEntity);
        Task<bool> Edit(MemberDetails member);
        Task<Member> GetDetails(int id);
        Task<List<Member>> GetList();
        Task<bool> UpdateStatus(int bookId, string user);
        Task<bool> Delete(int memberId);
    }
}
