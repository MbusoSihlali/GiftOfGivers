using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfGivers.Models
{
    public class Donation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string DonorName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string DonorEmail { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "ZAR";

        [Required]
        [StringLength(50)]
        public string DonationType { get; set; } = "One-Time"; // One-Time, Recurring

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed

        [StringLength(20)]
        public string ReceiptNumber { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}