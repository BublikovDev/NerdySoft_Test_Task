using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data
{
    public class AnnouncementDbContext : DbContext
    {
        public AnnouncementDbContext(DbContextOptions<AnnouncementDbContext> options) : base(options)
        {
        }

        public DbSet<Announcement> Announcements { get; set; }
    }
}
