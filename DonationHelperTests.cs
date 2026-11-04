using Xunit;
using GiftOfGivers.Helpers;

namespace GiftOfGivers.Test
{
    public class DonationHelperTests
    {
        [Fact]
        public void CalculateNetDonation_WithValidAmount_AppliesFeeCorrectly()
        {
            // Arrange
            decimal grossAmount = 1000.00m;
            decimal expectedNet = 975.00m; // 2.5% fee applied

            // Act
            decimal actualNet = DonationHelpers.CalculateNetDonation(grossAmount, 2.5m);

            // Assert
            Assert.Equal(expectedNet, actualNet);
        }

        [Fact]
        public void CalculateNetDonation_WithZeroOrNegativeAmount_ReturnsZero()
        {
            // Arrange
            decimal invalidAmount = -50.00m;

            // Act
            decimal actualNet = DonationHelpers.CalculateNetDonation(invalidAmount);

            // Assert
            Assert.Equal(0.00m, actualNet);
        }

        [Fact]
        public void GenerateTaxCertificateNumber_ReturnsFormattedString()
        {
            // Arrange
            int donationId = 42;
            var donationDate = new DateTime(2026, 10, 21);
            string expectedFormat = "GOTG-TAX-202610-000042";

            // Act
            string actualTaxCert = DonationHelpers.GenerateTaxCertificateNumber(donationId, donationDate);

            // Assert
            Assert.Equal(expectedFormat, actualTaxCert);
        }
    }
}