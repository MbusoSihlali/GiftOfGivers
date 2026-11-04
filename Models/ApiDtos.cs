namespace GiftOfGivers.Models
{
    public class StatusUpdateDto
    {
        public string Status { get; set; } = string.Empty;
    }

    public class DonationCreateDto
    {
        public string DonorName { get; set; } = string.Empty;
        public string DonorEmail { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "ZAR";
        public string DonationType { get; set; } = "One-Time";
        public int? ProjectId { get; set; }
    }

    public class VolunteerCreateDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Skills { get; set; } = string.Empty;
        public string Availability { get; set; } = string.Empty;
        public string Motivation { get; set; } = string.Empty;
    }
}