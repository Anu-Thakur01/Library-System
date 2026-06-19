using LibrarySystem.Repository.Models;
using LibrarySystem.Shared.AuthorData;

namespace LibrarySystem.Repository.AuthorRepository
{
    public interface IAuthorRepository
    {
        Task<bool> Add(Author authorEntity);
        Task<bool> Edit(AuthorDetails author);  
        Task<Author> GetDetails(int id);
        Task<List<Author>> GetList();
        Task<bool> UpdateStatus(int authorId, string user);
        Task<bool> Delete(int authorId);
    }
}
