using Server.Models;

namespace Server.Services.AnnouncementService
{
    public interface IAnnouncementRepository
    {
        IEnumerable<Announcement> GetAll();
        Announcement GetById(int id);
        void Add(Announcement announcement);
        void Update(Announcement announcement);
        void Delete(int id);
    }
}
