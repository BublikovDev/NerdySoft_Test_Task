namespace Server.Models.Responses
{
    public class AnnouncementDetails
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public ICollection<Announcement> SimilarAnnouncements { get; set; }
    }
}
