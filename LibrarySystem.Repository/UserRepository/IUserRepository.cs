using LibrarySystem.Shared.UserData;

namespace LibrarySystem.Repository.UserRepository
{
    public interface IUserRepository
    {
        Task<UserOperationResult> Add(UserCreateDetails userDetails);
        Task<UserOperationResult> Edit(UserEditDetails userDetails);
        Task<UserEditDetails?> GetDetails(string id);
        Task<List<UserListDetails>> GetList();
        Task<List<string>> GetRoles();
        Task<bool> IsUsernameAvailable(string userName);
    }
}
