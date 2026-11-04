
    using GiftOfGivers.Helpers;
    using GiftOfGivers.Models;
    using global::GiftOfGivers.Helpers;
    using Xunit;

    namespace GiftOfGivers.Test
    {
        public class DonationIntegrationTests
        {
            [Fact]
            public void DonationWorkflow_IntegrationTest_ValidatesHelpersAndModelFlow()
            {
                // Arrange: Simulate incoming donation form input data
                int testProjectId = 101;
                decimal rawDonationAmount = 2500.00m;
                var donationTimestamp = DateTime.UtcNow;

                // Act 1: Invoke NuGet package utility methods (simulating controller workflow)
                decimal calculatedNet = DonationHelpers.CalculateNetDonation(rawDonationAmount, 2.5m);
                string generatedTaxCert = DonationHelpers.GenerateTaxCertificateNumber(testProjectId, donationTimestamp);

                // Act 2: Map to Donation model (simulating database entity preparation)
                var donationEntity = new Donation
                {
                    Id = 1,
                    DonorName = "Integration Test Donor",
                    DonorEmail = "donor@test.com",
                    Amount = rawDonationAmount,
                    Currency = "ZAR",
                    DonationType = "Disaster Relief",
                    Status = "Completed",
                    CreatedAt = donationTimestamp
                };

                // Assert: Verify that the complete integration flow behaves correctly and data is cohesive
                Assert.Equal(2437.50m, calculatedNet); // 2500 - 2.5% fee
                Assert.StartsWith("GOTG-TAX-", generatedTaxCert);
                Assert.Contains("2026", generatedTaxCert);

                // Validate entity model attributes
                Assert.NotNull(donationEntity);
                Assert.Equal("Completed", donationEntity.Status);
                Assert.Equal(2500.00m, donationEntity.Amount);
            }
        }
    }
  
