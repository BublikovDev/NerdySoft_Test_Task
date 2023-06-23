using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Models.Requests;
using Server.Models.Responses;
using Server.Services.AnnouncementService;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/announcements")]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementRepository _announcementRepository;

        public AnnouncementController(IAnnouncementRepository announcementRepository)
        {
            _announcementRepository = announcementRepository;
        }

        [HttpPost]
        public IActionResult CreateAnnouncement([FromBody] CreateAnnouncementRequest request)
        {
            try
            {
                var new_Announcement = new Announcement()
                {
                    Title = request.Title,
                    Description = request.Description,
                    DateAdded = DateTime.UtcNow,
                    DateLastUpdated = DateTime.UtcNow
                };
                _announcementRepository.Add(new_Announcement);
                return CreatedAtAction(nameof(GetAnnouncementById), new { id = new_Announcement.Id }, new_Announcement);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAllAnnouncements()
        {
            try
            {
                var announcements = _announcementRepository.GetAll();
                if (announcements == null || announcements.Count() < 1)
                    return NotFound();
                return Ok(announcements);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetAnnouncementById(int id)
        {
            try
            {
                var announcement = _announcementRepository.GetById(id);
                if (announcement == null)
                {
                    return NotFound();
                }
                return Ok(announcement);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/details")]
        public IActionResult GetAnnouncementDetails(int id)
        {
            try
            {
                var announcement = _announcementRepository.GetById(id);
                if (announcement == null)
                {
                    return NotFound();
                }

                var similarAnnouncements = _announcementRepository.GetAll()
                    .Where(a => a.Id != id && (ContainsSharedWord(a.Title, announcement.Title) && ContainsSharedWord(a.Description, announcement.Description)))
                    .Take(3)
                    .ToList();

                AnnouncementDetails announcementDetails = new()
                {
                    Title = announcement.Title,
                    Description = announcement.Description,
                    DateAdded = announcement.DateAdded,
                    DateLastUpdated = announcement.DateLastUpdated,
                    SimilarAnnouncements = similarAnnouncements
                };

                return Ok(announcementDetails);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAnnouncement(int id, [FromBody] UpdateAnnouncementRequest request)
        {
            try
            {
                var existingAnnouncement = _announcementRepository.GetById(id);
                if (existingAnnouncement == null)
                {
                    return NotFound();
                }
                var announcementToUpdate = new Announcement()
                {
                    Id = id,
                    Title = request.Title,
                    Description = request.Description,
                    DateAdded = existingAnnouncement.DateAdded,
                    DateLastUpdated = DateTime.UtcNow,
                };
                _announcementRepository.Update(announcementToUpdate);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAnnouncement(int id)
        {
            try
            {
                var existingAnnouncement = _announcementRepository.GetById(id);
                if (existingAnnouncement == null)
                {
                    return NotFound();
                }
                _announcementRepository.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool ContainsSharedWord(string text1, string text2)
        {
            var words1 = text1.Split(new char[] { ' ', ',', '.', ':', ';', '-', '?' }, StringSplitOptions.RemoveEmptyEntries);
            var words2 = text2.Split(new char[] { ' ', ',', '.', ':', ';', '-', '?' }, StringSplitOptions.RemoveEmptyEntries);

            return words1.Intersect(words2, StringComparer.OrdinalIgnoreCase).Any();
        }
    }
}
