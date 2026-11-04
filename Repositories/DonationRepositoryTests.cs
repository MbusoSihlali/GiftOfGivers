using System.Linq;
using Microsoft.EntityFrameworkCore;
using GiftOfGivers.Data;
using GiftOfGivers.Models;
using GiftOfGivers.Repositories;
using Xunit;

namespace GiftOfGivers.Mvc.Tests.Repositories
{
    public class DonationRepositoryTests
    {
        private ApplicationDbContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task AddAsync_ShouldAddDonationToDatabase()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new DonationRepository(context);
            var donation = new Donation
            {
                DonorName = "Test Donor",
                DonorEmail = "test@example.com",
                Amount = 100,
                Currency = "ZAR",
                DonationType = "One-Time",
                Status = "Completed",
                ReceiptNumber = "TEST-001"
            };

            // Act
            await repository.AddAsync(donation);

            // Assert
            var savedDonation = await context.Donations.FirstOrDefaultAsync(d => d.DonorName == "Test Donor");
            Assert.NotNull(savedDonation);
            Assert.Equal(100, savedDonation.Amount);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectDonation()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new DonationRepository(context);
            var donation = new Donation
            {
                DonorName = "Test Donor",
                DonorEmail = "test@example.com",
                Amount = 100,
                Currency = "ZAR",
                DonationType = "One-Time",
                Status = "Completed",
                ReceiptNumber = "TEST-001"
            };
            await repository.AddAsync(donation);

            // Act
            var retrievedDonation = await repository.GetByIdAsync(donation.Id);

            // Assert
            Assert.NotNull(retrievedDonation);
            Assert.Equal("Test Donor", retrievedDonation.DonorName);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllDonations()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new DonationRepository(context);
            
            await repository.AddAsync(new Donation
            {
                DonorName = "Donor 1",
                DonorEmail = "donor1@example.com",
                Amount = 100,
                Currency = "ZAR",
                DonationType = "One-Time",
                Status = "Completed",
                ReceiptNumber = "TEST-001"
            });
            
            await repository.AddAsync(new Donation
            {
                DonorName = "Donor 2",
                DonorEmail = "donor2@example.com",
                Amount = 200,
                Currency = "ZAR",
                DonationType = "One-Time",
                Status = "Completed",
                ReceiptNumber = "TEST-002"
            });

            // Act
            var donations = await repository.GetAllAsync();

            // Assert
            Assert.Equal(2, donations.Count());
        }

        [Fact]
        public async Task GetTotalDonationsAsync_ShouldReturnSumOfCompletedDonations()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new DonationRepository(context);
            
            await repository.AddAsync(new Donation
            {
                DonorName = "Donor 1",
                DonorEmail = "donor1@example.com",
                Amount = 100,
                Currency = "ZAR",
                DonationType = "One-Time",
                Status = "Completed",
                ReceiptNumber = "TEST-001"
            });
            
            await repository.AddAsync(new Donation
            {
                DonorName = "Donor 2",
                DonorEmail = "donor2@example.com",
                Amount = 200,
                Currency = "ZAR",
                DonationType = "One-Time",
                Status = "Pending",
                ReceiptNumber = "TEST-002"
            });

            // Act
            var total = await repository.GetTotalDonationsAsync();

            // Assert
            Assert.Equal(100, total); // Only completed donations
        }

        [Fact]
        public async Task GetRecentDonationsAsync_ShouldReturnLimitedNumberOfDonations()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new DonationRepository(context);
            
            for (int i = 1; i <= 10; i++)
            {
                await repository.AddAsync(new Donation
                {
                    DonorName = $"Donor {i}",
                    DonorEmail = $"donor{i}@example.com",
                    Amount = 100 * i,
                    Currency = "ZAR",
                    DonationType = "One-Time",
                    Status = "Completed",
                    ReceiptNumber = $"TEST-{i:000}"
                });
            }

            // Act
            var recentDonations = await repository.GetRecentDonationsAsync(5);

            // Assert
            Assert.Equal(5, recentDonations.Count());
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateDonation()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new DonationRepository(context);
            var donation = new Donation
            {
                DonorName = "Test Donor",
                DonorEmail = "test@example.com",
                Amount = 100,
                Currency = "ZAR",
                DonationType = "One-Time",
                Status = "Completed",
                ReceiptNumber = "TEST-001"
            };
            await repository.AddAsync(donation);

            // Act
            donation.Amount = 500;
            await repository.UpdateAsync(donation);

            // Assert
            var updatedDonation = await repository.GetByIdAsync(donation.Id);
            Assert.Equal(500, updatedDonation.Amount);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveDonation()
        {
            // Arrange
            var context = GetInMemoryContext();
            var repository = new DonationRepository(context);
            var donation = new Donation
            {
                DonorName = "Test Donor",
                DonorEmail = "test@example.com",
                Amount = 100,
                Currency = "ZAR",
                DonationType = "One-Time",
                Status = "Completed",
                ReceiptNumber = "TEST-001"
            };
            await repository.AddAsync(donation);

            // Act
            await repository.DeleteAsync(donation);

            // Assert
            var deletedDonation = await repository.GetByIdAsync(donation.Id);
            Assert.Null(deletedDonation);
        }
    }
}