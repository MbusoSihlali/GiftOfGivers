using System.ComponentModel.DataAnnotations;

namespace GiftOfGivers.Models
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        [Display(Name = "City")]
        public string? City { get; set; }

        [StringLength(100)]
        [Display(Name = "Province")]
        public string? Province { get; set; }

        [StringLength(255)]
        [Display(Name = "Address")]
        public string? Address { get; set; }
    }
}