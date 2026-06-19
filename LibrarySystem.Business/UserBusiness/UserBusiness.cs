using LibrarySystem.Repository.UserRepository;
using LibrarySystem.Shared.UserData;

namespace LibrarySystem.Business.UserBusiness
{
    public class UserBusiness : IUserBusiness
    {
        private readonly IUserRepository _userRepository;

        public UserBusiness(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserOperationResult> Add(UserCreateDetails userDetails)
        {
            return await _userRepository.Add(userDetails);
        }

        public async Task<UserOperationResult> Edit(UserEditDetails userDetails)
        {
            return await _userRepository.Edit(userDetails);
        }

        public async Task<UserEditDetails?> GetDetails(string id)
        {
            return await _userRepository.GetDetails(id);
        }

        public async Task<List<UserListDetails>> GetList()
        {
            return await _userRepository.GetList();
        }

        public async Task<List<string>> GetRoles()
        {
            return await _userRepository.GetRoles();
        }

        public async Task<bool> IsUsernameAvailable(string userName)
        {
            return await _userRepository.IsUsernameAvailable(userName);
        }
    }
}
