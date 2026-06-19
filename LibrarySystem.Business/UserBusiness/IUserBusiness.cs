using LibrarySystem.Shared.UserData;

namespace LibrarySystem.Business.UserBusiness
{
    public interface IUserBusiness
    {
        Task<UserOperationResult> Add(UserCreateDetails userDetails);
        Task<UserOperationResult> Edit(UserEditDetails userDetails);
        Task<UserEditDetails?> GetDetails(string id);
        Task<List<UserListDetails>> GetList();
        Task<List<string>> GetRoles();
        Task<bool> IsUsernameAvailable(string userName);
    }
}
