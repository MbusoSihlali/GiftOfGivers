using System.Collections.Generic;

namespace GiftOfGivers.Models
{
    public class DashboardViewModel
    {
        public decimal TotalDonations { get; set; }
        public int PendingVolunteerCount { get; set; }
        public int ActiveProjectCount { get; set; }
        public List<Donation> RecentDonations { get; set; } = new List<Donation>();
        public List<Volunteer> PendingVolunteers { get; set; } = new List<Volunteer>();
        public List<Project> ActiveProjects { get; set; } = new List<Project>();
    }
}