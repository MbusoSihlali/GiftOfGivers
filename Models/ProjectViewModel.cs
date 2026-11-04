using System.ComponentModel.DataAnnotations;

namespace GiftOfGivers.Models
{
    public class ProjectViewModel
    {
        [Required(ErrorMessage = "Project title is required.")]
        [StringLength(200)]
        [Display(Name = "Project Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(100)]
        [Display(Name = "Category")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(200)]
        [Display(Name = "Location")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Target amount is required.")]
        [Range(1000, 10000000, ErrorMessage = "Target amount must be between 1,000 and 10,000,000")]
        [Display(Name = "Target Amount")]
        public decimal TargetAmount { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [StringLength(1000)]
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }
    }
}