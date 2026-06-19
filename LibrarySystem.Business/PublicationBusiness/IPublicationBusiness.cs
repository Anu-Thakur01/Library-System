using LibrarySystem.Shared.PublicationData;

namespace LibrarySystem.Business.PublicationBusiness
{
    public interface IPublicationBusiness
    {
        Task<bool> Add(PublicationDetails publication);
        Task<bool> Edit(PublicationDetails publication);
        Task<PublicationDetails> GetDetails(int id);
        Task<List<PublicationDetails>> GetList();
        Task<bool> UpdateStatus(int publicationId, string user);
        Task<bool> Delete(int publicationId);
    }
}
