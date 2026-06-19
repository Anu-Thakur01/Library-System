using LibrarySystem.Repository.Data;
using LibrarySystem.Repository.Models;
using LibrarySystem.Shared.PublicationData;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Repository.PublicationRepository
{
    public class PublicationRepository : IPublicationRepository
    {
        private readonly ApplicationDbContext _context;

        public PublicationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Add(Publication data)
        {
            await _context.Publication.AddAsync(data);
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

        public async Task<bool> Edit(PublicationDetails publication)
        {
            var details = _context.Publication.FirstOrDefault(x => x.PublicationId == publication.PublicationId);
            if (details != null)
            {
                details.PublicationName = publication.PublicationName;
                details.PublicationAddress = publication.PublicationAddress;
                details.ContactPersonName = publication.ContactPersonName;
                details.ContactPhone = publication.ContactPhone;
                details.PublicationEmail = publication.PublicationEmail;
                details.PublicationWebsite = publication.PublicationWebsite;
                details.Status = publication.Status;
                details.ModifiedBy = publication.User;
                details.ModifiedDate = DateTime.Now;
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return true;
            }
            return false;
        }

        public async Task<bool> Delete(int publicationId)
        {
            var details = await _context.Publication.FirstOrDefaultAsync(x => x.PublicationId == publicationId);
            if (details != null)
            {
                _context.Publication.Remove(details);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                    return true;
            }
            return false;
        }

        public async Task<Publication> GetDetails(int id)
        {
            var details = await _context.Publication.AsNoTracking().FirstOrDefaultAsync(x => x.PublicationId == id);
            return details;
        }

        public async Task<List<Publication>> GetList()
        {
            var listValue = await _context.Publication.AsNoTracking().OrderByDescending(x => x.PublicationId).ToListAsync();
            return listValue;
        }

        public async Task<bool> UpdateStatus(int publicationId, string user)
        {
            var details = await _context.Publication.FirstOrDefaultAsync(x => x.PublicationId == publicationId);
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
