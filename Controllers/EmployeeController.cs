using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GiftOfGivers.Models;
using GiftOfGivers.Repositories;

namespace GiftOfGivers.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly IDonationRepository _donationRepository;
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(
            IDonationRepository donationRepository,
            IVolunteerRepository volunteerRepository,
            IProjectRepository projectRepository,
            ILogger<EmployeeController> logger)
        {
            _donationRepository = donationRepository;
            _volunteerRepository = volunteerRepository;
            _projectRepository = projectRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalDonations = await _donationRepository.GetTotalDonationsAsync();
            var pendingVolunteers = await _volunteerRepository.GetVolunteersByStatusAsync("Pending");
            var activeProjects = await _projectRepository.GetActiveProjectsAsync();
            var recentDonations = await _donationRepository.GetRecentDonationsAsync(10);

            var dashboardData = new DashboardViewModel
            {
                TotalDonations = totalDonations,
                PendingVolunteerCount = pendingVolunteers.Count(),
                ActiveProjectCount = activeProjects.Count(),
                RecentDonations = recentDonations.ToList(),
                PendingVolunteers = pendingVolunteers.ToList(),
                ActiveProjects = activeProjects.ToList()
            };

            return View(dashboardData);
        }

        public async Task<IActionResult> Donations()
        {
            var donations = await _donationRepository.GetAllAsync();
            return View(donations.OrderByDescending(d => d.CreatedAt));
        }

        public async Task<IActionResult> Volunteers()
        {
            var volunteers = await _volunteerRepository.GetAllAsync();
            return View(volunteers.OrderByDescending(v => v.CreatedAt));
        }

        public async Task<IActionResult> VolunteerDetails(int id)
        {
            var volunteer = await _volunteerRepository.GetByIdAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }
            return View(volunteer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveVolunteer(int id)
        {
            var volunteer = await _volunteerRepository.GetByIdAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }

            volunteer.Status = "Approved";
            volunteer.UpdatedAt = DateTime.UtcNow;
            await _volunteerRepository.UpdateAsync(volunteer);

            _logger.LogInformation("Volunteer {Id} approved by employee", id);
            TempData["SuccessMessage"] = "Volunteer application approved successfully.";
            return RedirectToAction("Volunteers");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectVolunteer(int id)
        {
            var volunteer = await _volunteerRepository.GetByIdAsync(id);
            if (volunteer == null)
            {
                return NotFound();
            }

            volunteer.Status = "Rejected";
            volunteer.UpdatedAt = DateTime.UtcNow;
            await _volunteerRepository.UpdateAsync(volunteer);

            _logger.LogInformation("Volunteer {Id} rejected by employee", id);
            TempData["SuccessMessage"] = "Volunteer application rejected.";
            return RedirectToAction("Volunteers");
        }

        public async Task<IActionResult> Projects()
        {
            var projects = await _projectRepository.GetAllAsync();
            return View(projects.OrderByDescending(p => p.CreatedAt));
        }

        [HttpGet]
        public IActionResult CreateProject()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject(ProjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var project = new Project
            {
                Title = model.Title,
                Category = model.Category,
                Description = model.Description,
                Location = model.Location,
                TargetAmount = model.TargetAmount,
                RaisedAmount = 0,
                Status = "Active",
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                ImageUrl = model.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            await _projectRepository.AddAsync(project);
            _logger.LogInformation("Project {Title} created by employee", model.Title);

            TempData["SuccessMessage"] = "Project created successfully.";
            return RedirectToAction("Projects");
        }

        public async Task<IActionResult> ProjectDetails(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }
    }
}