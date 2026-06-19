using LibrarySystem.Repository.Models;
using LibrarySystem.Shared.CategoryData;

namespace LibrarySystem.Repository.CategoryRepository
{
    public interface ICategoryRepository
    {
        Task<bool> Add(Category categoryEntity);
        Task<bool> Edit(CategoryDetails category);
        Task<Category> GetDetails(int id);
        Task<List<Category>> GetList();
        Task<bool> UpdateStatus(int bookId, string user);
        Task<bool> Delete(int categoryId);
    }
}
