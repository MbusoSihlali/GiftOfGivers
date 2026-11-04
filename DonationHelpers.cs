using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiftOfGivers.Helpers
{
    public static class DonationHelpers
    {
        /// <summary>
        /// Generates a standardized Section 18A Tax Certificate reference.
        /// </summary>
        public static string GenerateTaxCertificateNumber(int donationId, DateTime donationDate)
        {
            return $"GOTG-TAX-{donationDate:yyyyMM}-{donationId:D6}";
        }

        /// <summary>
        /// Calculates the net donation amount after applying standard transaction/processing fee percentages.
        /// </summary>
        public static decimal CalculateNetDonation(decimal grossAmount, decimal feePercentage = 2.5m)
        {
            if (grossAmount <= 0) return 0m;
            decimal fee = grossAmount * (feePercentage / 100m);
            return Math.Round(grossAmount - fee, 2);
        }
    }
}