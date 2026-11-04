using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using GiftOfGivers.Controllers;
using GiftOfGivers.Repositories;
using GiftOfGivers.Models;
using Xunit;
using System.Collections.Generic;

namespace GiftOfGivers.Mvc.Tests.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public async Task Index_ShouldReturnView()
        {
            // Arrange
            var mockProjectRepo = new Mock<IProjectRepository>();
            var mockDonationRepo = new Mock<IDonationRepository>();
            var mockLogger = new Mock<ILogger<HomeController>>();
            
            mockProjectRepo.Setup(repo => repo.GetActiveProjectsAsync()).ReturnsAsync(new List<Project>());
            mockDonationRepo.Setup(repo => repo.GetTotalDonationsAsync()).ReturnsAsync(1000);
            mockDonationRepo.Setup(repo => repo.GetRecentDonationsAsync(It.IsAny<int>())).ReturnsAsync(new List<Donation>());
            
            var controller = new HomeController(mockLogger.Object, mockProjectRepo.Object, mockDonationRepo.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void About_ShouldReturnView()
        {
            // Arrange
            var mockProjectRepo = new Mock<IProjectRepository>();
            var mockDonationRepo = new Mock<IDonationRepository>();
            var mockLogger = new Mock<ILogger<HomeController>>();
            
            var controller = new HomeController(mockLogger.Object, mockProjectRepo.Object, mockDonationRepo.Object);

            // Act
            var result = controller.About();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Contact_ShouldReturnView()
        {
            // Arrange
            var mockProjectRepo = new Mock<IProjectRepository>();
            var mockDonationRepo = new Mock<IDonationRepository>();
            var mockLogger = new Mock<ILogger<HomeController>>();
            
            var controller = new HomeController(mockLogger.Object, mockProjectRepo.Object, mockDonationRepo.Object);

            // Act
            var result = controller.Contact("John", "john@example.com", "Test", "Test message");

            // Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void Privacy_ShouldReturnView()
        {
            // Arrange
            var mockProjectRepo = new Mock<IProjectRepository>();
            var mockDonationRepo = new Mock<IDonationRepository>();
            var mockLogger = new Mock<ILogger<HomeController>>();
            
            var controller = new HomeController(mockLogger.Object, mockProjectRepo.Object, mockDonationRepo.Object);

            // Act
            var result = controller.Privacy();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error_ShouldReturnView()
        {
            // Arrange
            var mockProjectRepo = new Mock<IProjectRepository>();
            var mockDonationRepo = new Mock<IDonationRepository>();
            var mockLogger = new Mock<ILogger<HomeController>>();
            
            var controller = new HomeController(mockLogger.Object, mockProjectRepo.Object, mockDonationRepo.Object);

            // Act
            var result = controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }
    }
}