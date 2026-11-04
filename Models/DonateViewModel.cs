using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace GiftOfGivers.Models
{
    public class DonateViewModel
    {
        [StringLength(100)]
        [Display(Name = "Donor Name (Leave blank for Anonymous)")]
        public string DonorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string DonorEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 1000000, ErrorMessage = "Amount must be between 0.01 and 1,000,000")]
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Currency is required.")]
        [Display(Name = "Currency")]
        public string Currency { get; set; } = "ZAR";

        [Required(ErrorMessage = "Donation type is required.")]
        [Display(Name = "Frequency")]
        public string DonationType { get; set; } = "One-Time";

        [Display(Name = "Allocate to Project")]
        public int? ProjectId { get; set; }

        public List<Project>? Projects { get; set; }
    }
}