using System.ComponentModel.DataAnnotations;

namespace GiftOfGivers.Models
{
    public class VolunteerViewModel
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100)]
        [Display(Name = "City")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Province is required.")]
        [StringLength(100)]
        [Display(Name = "Province")]
        public string Province { get; set; } = string.Empty;

        [Required(ErrorMessage = "Skills are required.")]
        [StringLength(500)]
        [Display(Name = "Skills (e.g., Medical, Logistics, General)")]
        public string Skills { get; set; } = string.Empty;

        [Required(ErrorMessage = "Availability information is required.")]
        [StringLength(500)]
        [Display(Name = "Availability")]
        public string Availability { get; set; } = string.Empty;

        [Required(ErrorMessage = "Motivation is required.")]
        [StringLength(1000)]
        [Display(Name = "Why do you want to volunteer?")]
        public string Motivation { get; set; } = string.Empty;
    }
}