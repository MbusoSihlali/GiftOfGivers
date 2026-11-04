using Microsoft.AspNetCore.Mvc;
using GiftOfGivers.Repositories;

namespace GiftOfGivers.Controllers
{
    // Trigger CI Build Pipeline Test
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProjectRepository _projectRepository;
        private readonly IDonationRepository _donationRepository;

        public HomeController(
            ILogger<HomeController> logger,
            IProjectRepository projectRepository,
            IDonationRepository donationRepository)
        {
            _logger = logger;
            _projectRepository = projectRepository;
            _donationRepository = donationRepository;
        }

        public async Task<IActionResult> Index()
        {
            var activeProjects = await _projectRepository.GetActiveProjectsAsync();
            var totalRaised = await _donationRepository.GetTotalDonationsAsync();
            var recentDonations = await _donationRepository.GetRecentDonationsAsync(5);

            ViewBag.ActiveProjects = activeProjects;
            ViewBag.TotalRaised = totalRaised;
            ViewBag.RecentDonations = recentDonations;

            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(string name, string email, string subject, string message)
        {
            // In a real application, this would send an email or save to database
            // For now, we'll just redirect back with a success message
            TempData["ContactSuccess"] = "Thank you for your message. We'll get back to you soon!";
            return RedirectToAction("Contact");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}