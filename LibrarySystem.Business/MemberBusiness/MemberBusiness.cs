using LibrarySystem.Repository.MemberRepository;
using LibrarySystem.Repository.Models;
using LibrarySystem.Shared.MemberData;

namespace LibrarySystem.Business.MemberBusiness
{
    public class MemberBusiness : IMemberBusiness
    {
        private readonly IMemberRepository _memberRepository;

        public MemberBusiness(IMemberRepository publicationRepository)
        {
            _memberRepository = publicationRepository;
        }

        public async Task<bool> Add(MemberDetails memberDetails)
        {
            var memberEntity = new Member
            {
                MemberName = memberDetails.MemberName,
                PhoneNumber = memberDetails.PhoneNumber,
                Address = memberDetails.Address,
                Email = memberDetails.Email,
                JoinedDate = memberDetails.JoinedDate,
                ExpirationDate = memberDetails.JoinedDate.AddDays(15),
                MembershipType = memberDetails.MembershipType,
                Status = string.IsNullOrWhiteSpace(memberDetails.Status) ? "A" : memberDetails.Status,
                CreatedBy = memberDetails.User,
                CreatedDate = DateTime.Now
            };
            return await _memberRepository.Add(memberEntity);
        }

        public async Task<bool> Edit(MemberDetails publication)
        {
            return await _memberRepository.Edit(publication);
        }

        public async Task<bool> Delete(int memberId)
        {
            return await _memberRepository.Delete(memberId);
        }

        public async Task<MemberDetails> GetDetails(int id)
        {
            var memberData = await _memberRepository.GetDetails(id);
            var memberDetails = new MemberDetails
            {
                MemberId = memberData.MemberId,
                MemberName = memberData.MemberName,
                PhoneNumber = memberData.PhoneNumber,
                Address = memberData.Address,
                Email = memberData.Email,
                JoinedDate = memberData.JoinedDate,
                MembershipType = memberData.MembershipType,
                Status = memberData.Status
            };
            return memberDetails;
        }

        public async Task<List<MemberDetails>> GetList()
        {
            List<MemberDetails> memberList = new List<MemberDetails>();
            var members = await _memberRepository.GetList();
            foreach (var member in members)
            {
                memberList.Add(new MemberDetails
                {
                    MemberId = member.MemberId,
                    MemberName = member.MemberName,
                    PhoneNumber = member.PhoneNumber,
                    Address = member.Address,
                    Email = member.Email,
                    JoinedDate = member.JoinedDate,
                    MembershipType = member.MembershipType,
                    Status = string.IsNullOrEmpty(member.Status) ? "A" : member.Status,
                });
            }
            return memberList;
        }

        public async Task<bool> UpdateStatus(int memberId, string user)
        {
            var result = await _memberRepository.UpdateStatus(memberId, user);
            return result;
        }
    }
}
