using Microsoft.AspNetCore.Mvc;
using Moq;
using Server.Controllers;
using Server.Models;
using Server.Models.Requests;
using Server.Models.Responses;
using Server.Services.AnnouncementService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class AnnouncementControllerTests
    {
        private AnnouncementController _controller;
        private Mock<IAnnouncementRepository> _repositoryMock;


        public AnnouncementControllerTests()
        {
            _repositoryMock = new Mock<IAnnouncementRepository>();
            _controller = new AnnouncementController(_repositoryMock.Object);
        }

        [Fact]
        public void CreateAnnouncement_ValidRequest_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var request = new CreateAnnouncementRequest
            {
                Title = "Test Announcement",
                Description = "This is a test announcement."
            };
            var newAnnouncement = new Announcement
            {
                Id = 1,
                Title = "Test Announcement",
                Description = "This is a test announcement.",
                DateAdded = DateTime.UtcNow,
                DateLastUpdated = DateTime.UtcNow
            };
            _repositoryMock.Setup(r => r.Add(It.IsAny<Announcement>())).Callback<Announcement>(announcement =>
            {
                announcement.Id = newAnnouncement.Id;
                announcement.DateAdded = newAnnouncement.DateAdded;
                announcement.DateLastUpdated = newAnnouncement.DateLastUpdated;
            });

            // Act
            var result = _controller.CreateAnnouncement(request);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdAnnouncement = Assert.IsAssignableFrom<Announcement>(createdAtActionResult.Value);
            Assert.Equal(newAnnouncement.Id, createdAnnouncement.Id);
            Assert.Equal(newAnnouncement.DateAdded, createdAnnouncement.DateAdded);
            Assert.Equal(newAnnouncement.DateLastUpdated, createdAnnouncement.DateLastUpdated);
        }

        [Fact]
        public void CreateAnnouncement_ExceptionThrown_ReturnsBadRequestResult()
        {
            // Arrange
            var request = new CreateAnnouncementRequest
            {
                Title = "Test Announcement",
                Description = "This is a test announcement."
            };
            var exceptionMessage = "An error occurred while creating the announcement.";
            _repositoryMock.Setup(r => r.Add(It.IsAny<Announcement>())).Throws(new Exception(exceptionMessage));

            // Act
            var result = _controller.CreateAnnouncement(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(exceptionMessage, badRequestResult.Value);
        }

        [Fact]
        public void GetAllAnnouncements_NoAnnouncements_ReturnsNotFoundResult()
        {
            // Arrange
            var emptyList = Enumerable.Empty<Announcement>();
            _repositoryMock.Setup(r => r.GetAll()).Returns(emptyList);

            // Act
            var result = _controller.GetAllAnnouncements();

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetAllAnnouncements_AnnouncementsExist_ReturnsOkResultWithAnnouncements()
        {
            // Arrange
            var announcements = new List<Announcement>
        {
            new Announcement { Id = 1, Title = "Announcement 1", Description = "Description 1" },
            new Announcement { Id = 2, Title = "Announcement 2", Description = "Description 2" }
        };
            _repositoryMock.Setup(r => r.GetAll()).Returns(announcements);

            // Act
            var result = _controller.GetAllAnnouncements();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAnnouncements = Assert.IsAssignableFrom<IEnumerable<Announcement>>(okResult.Value);
            Assert.Equal(announcements.Count, returnedAnnouncements.Count());
        }

        [Fact]
        public void GetAnnouncementById_ExistingId_ReturnsOkResultWithAnnouncement()
        {
            // Arrange
            var existingId = 1;
            var expectedAnnouncement = new Announcement
            {
                Id = existingId,
                Title = "Test Announcement",
                Description = "This is a test announcement."
            };
            _repositoryMock.Setup(r => r.GetById(existingId)).Returns(expectedAnnouncement);

            // Act
            var result = _controller.GetAnnouncementById(existingId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var announcement = Assert.IsAssignableFrom<Announcement>(okResult.Value);
            Assert.Equal(expectedAnnouncement.Id, announcement.Id);
            Assert.Equal(expectedAnnouncement.Title, announcement.Title);
            Assert.Equal(expectedAnnouncement.Description, announcement.Description);
        }

        [Fact]
        public void GetAnnouncementById_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var nonExistingId = 10;
            _repositoryMock.Setup(r => r.GetById(nonExistingId)).Returns((Announcement)null);

            // Act
            var result = _controller.GetAnnouncementById(nonExistingId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetAnnouncementDetails_ExistingId_ReturnsOkResultWithAnnouncementDetails()
        {
            // Arrange
            var existingId = 1;
            var existingAnnouncement = new Announcement
            {
                Id = existingId,
                Title = "Test Announcement",
                Description = "This is a test announcement.",
                DateAdded = DateTime.UtcNow,
                DateLastUpdated = DateTime.UtcNow
            };
            var similarAnnouncements = new List<Announcement>
        {
            new Announcement { Id = 2, Title = "Similar Announcement 1", Description = "Description test 1" },
            new Announcement { Id = 3, Title = "Similar Announcement 2", Description = "Description test 2" }
        };
            _repositoryMock.Setup(r => r.GetById(existingId)).Returns(existingAnnouncement);
            _repositoryMock.Setup(r => r.GetAll()).Returns(similarAnnouncements);

            // Act
            var result = _controller.GetAnnouncementDetails(existingId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var announcementDetails = Assert.IsAssignableFrom<AnnouncementDetails>(okResult.Value);
            Assert.Equal(existingAnnouncement.Title, announcementDetails.Title);
            Assert.Equal(existingAnnouncement.Description, announcementDetails.Description);
            Assert.Equal(existingAnnouncement.DateAdded, announcementDetails.DateAdded);
            Assert.Equal(existingAnnouncement.DateLastUpdated, announcementDetails.DateLastUpdated);
            Assert.Equal(similarAnnouncements.Count, announcementDetails.SimilarAnnouncements.Count());
        }

        [Fact]
        public void GetAnnouncementDetails_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var nonExistingId = 10;
            _repositoryMock.Setup(r => r.GetById(nonExistingId)).Returns((Announcement)null);

            // Act
            var result = _controller.GetAnnouncementDetails(nonExistingId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void UpdateAnnouncement_ExistingId_ValidRequest_ReturnsNoContentResult()
        {
            // Arrange
            var existingId = 1;
            var existingAnnouncement = new Announcement
            {
                Id = existingId,
                Title = "Existing Announcement",
                Description = "Existing Description",
                DateAdded = DateTime.UtcNow,
                DateLastUpdated = DateTime.UtcNow
            };
            var updateRequest = new UpdateAnnouncementRequest
            {
                Title = "Updated Announcement",
                Description = "Updated Description"
            };
            _repositoryMock.Setup(r => r.GetById(existingId)).Returns(existingAnnouncement);

            // Act
            var result = _controller.UpdateAnnouncement(existingId, updateRequest);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void UpdateAnnouncement_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var nonExistingId = 10;
            var updateRequest = new UpdateAnnouncementRequest
            {
                Title = "Updated Announcement",
                Description = "Updated Description"
            };
            _repositoryMock.Setup(r => r.GetById(nonExistingId)).Returns((Announcement)null);

            // Act
            var result = _controller.UpdateAnnouncement(nonExistingId, updateRequest);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteAnnouncement_ExistingId_ReturnsNoContentResult()
        {
            // Arrange
            var existingId = 1;
            var existingAnnouncement = new Announcement
            {
                Id = existingId,
                Title = "Existing Announcement",
                Description = "Existing Description",
                DateAdded = DateTime.UtcNow,
                DateLastUpdated = DateTime.UtcNow
            };
            _repositoryMock.Setup(r => r.GetById(existingId)).Returns(existingAnnouncement);

            // Act
            var result = _controller.DeleteAnnouncement(existingId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteAnnouncement_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var nonExistingId = 10;
            _repositoryMock.Setup(r => r.GetById(nonExistingId)).Returns((Announcement)null);

            // Act
            var result = _controller.DeleteAnnouncement(nonExistingId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
