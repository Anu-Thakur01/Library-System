using LibrarySystem.Repository.Models;
using LibrarySystem.Shared.PublicationData;

namespace LibrarySystem.Repository.PublicationRepository
{
    public interface IPublicationRepository
    {
        Task<bool> Add(Publication publicationEntity);
        Task<bool> Edit(PublicationDetails publication);
        Task<Publication> GetDetails(int id);
        Task<List<Publication>> GetList();
        Task<bool> UpdateStatus(int bookId, string user);
        Task<bool> Delete(int publicationId);
    }
}
