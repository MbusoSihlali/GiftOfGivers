using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GiftOfGivers.Models;
using GiftOfGivers.Repositories;

namespace GiftOfGivers.Controllers
{

    public class VolunteerController : Controller
    {
        // Feature branch update: Implemented volunteer registration and activity tracking logic
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<VolunteerController> _logger;

        public VolunteerController(
            IVolunteerRepository volunteerRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<VolunteerController> logger)
        {
            _volunteerRepository = volunteerRepository;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(VolunteerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var volunteer = new Volunteer
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                City = model.City,
                Province = model.Province,
                Skills = model.Skills,
                Availability = model.Availability,
                Motivation = model.Motivation,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UserId = user?.Id
            };

            await _volunteerRepository.AddAsync(volunteer);
            _logger.LogInformation("Volunteer application submitted by {Name}", model.FullName);

            return RedirectToAction("Success");
        }

        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Applications()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var applications = await _volunteerRepository.FindAsync(v => v.UserId == user.Id);
            return View(applications);
        }
    }
}