using Server.Data;
using Server.Models;

namespace Server.Services.AnnouncementService
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly AnnouncementDbContext _dbContext;

        public AnnouncementRepository(AnnouncementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Announcement> GetAll()
        {
            return _dbContext.Announcements.ToList();
        }

        public Announcement GetById(int id)
        {
            return _dbContext.Announcements.FirstOrDefault(a => a.Id == id);
        }

        public void Add(Announcement announcement)
        {
            _dbContext.Announcements.Add(announcement);
            _dbContext.SaveChanges();
        }

        public void Update(Announcement announcement)
        {
            var announcementToUpdate = _dbContext.Announcements.Find(announcement.Id);
            announcementToUpdate.Title = announcement.Title;
            announcementToUpdate.Description = announcement.Description;
            announcementToUpdate.DateLastUpdated = announcement.DateLastUpdated;
            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var announcement = GetById(id);
            if (announcement != null)
            {
                _dbContext.Announcements.Remove(announcement);
                _dbContext.SaveChanges();
            }
        }
    }
}
