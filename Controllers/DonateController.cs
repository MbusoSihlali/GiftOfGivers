using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GiftOfGivers.Models;
using GiftOfGivers.Repositories;
using GiftOfGivers.Helpers; // Imported from published & installed NuGet package

namespace GiftOfGivers.Controllers
{
    // Feature branch update: Added Section 18A tax certificate integration via GiftOfGivers.Helpers NuGet Package
    public class DonateController : Controller
    {
        private readonly IDonationRepository _donationRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DonateController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public DonateController(
            IDonationRepository donationRepository,
            IProjectRepository projectRepository,
            UserManager<ApplicationUser> userManager,
            ILogger<DonateController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _donationRepository = donationRepository;
            _projectRepository = projectRepository;
            _userManager = userManager;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new DonateViewModel
            {
                Projects = new List<Project>()
            };

            try
            {
                if (_projectRepository != null)
                {
                    var projects = await _projectRepository.GetActiveProjectsAsync();
                    if (projects != null)
                    {
                        model.Projects = projects.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active projects for Donate page.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(DonateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var activeProjects = await _projectRepository.GetActiveProjectsAsync();
                model.Projects = activeProjects?.ToList() ?? new List<Project>();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            decimal netDonationAmount = DonationHelpers.CalculateNetDonation(model.Amount);
            string formattedTaxCertNo = DonationHelpers.GenerateTaxCertificateNumber(model.ProjectId ?? 0, DateTime.UtcNow);

            var donation = new Donation
            {
                DonorName = string.IsNullOrEmpty(model.DonorName) ? "Anonymous" : model.DonorName,
                DonorEmail = model.DonorEmail ?? string.Empty,
                Amount = model.Amount,
                Currency = string.IsNullOrEmpty(model.Currency) ? "ZAR" : model.Currency,
                DonationType = model.DonationType ?? "General",
                Status = "Completed",
                ReceiptNumber = GenerateReceiptNumber(),
                CreatedAt = DateTime.UtcNow,
                UserId = user?.Id
            };

            await _donationRepository.AddAsync(donation);
            _logger.LogInformation("Donation created: {Amount} {Currency} (Net: {NetAmount}) by {Donor}. Tax Cert Ref: {TaxRef}",
                model.Amount, model.Currency, netDonationAmount, model.DonorName, formattedTaxCertNo);

            // Store NuGet package generated values in TempData for display on the Success screen
            TempData["NetDonationAmount"] = netDonationAmount;
            TempData["TaxCertificateNumber"] = formattedTaxCertNo;

            // Update project raised amount if project is selected
            if (model.ProjectId.HasValue)
            {
                var project = await _projectRepository.GetByIdAsync(model.ProjectId.Value);
                if (project != null)
                {
                    project.RaisedAmount += model.Amount;
                    await _projectRepository.UpdateAsync(project);
                }
            }

            // Call Azure Function to generate dummy tax certificate
            try
            {
                var client = _httpClientFactory.CreateClient();
                var functionPayload = new
                {
                    donorName = donation.DonorName,
                    taxNumber = formattedTaxCertNo, // Using generated tax ref from NuGet package
                    amount = donation.Amount,
                    netAmount = netDonationAmount
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(functionPayload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync("http://localhost:7011/api/GenerateTaxCertificate", content);
                if (response.IsSuccessStatusCode)
                {
                    var certificateJson = await response.Content.ReadAsStringAsync();
                    TempData["TaxCertificateJson"] = certificateJson;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to call Azure Function for tax certificate generation.");
            }

            return RedirectToAction("Success", new { id = donation.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Success(int id)
        {
            var donation = await _donationRepository.GetByIdAsync(id);
            if (donation == null)
            {
                return NotFound();
            }

            ViewBag.TaxCertificateJson = TempData["TaxCertificateJson"] as string;
            ViewBag.NetDonationAmount = TempData["NetDonationAmount"];
            ViewBag.TaxCertificateNumber = TempData["TaxCertificateNumber"];

            return View(donation);
        }

        [Authorize]
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var donations = await _donationRepository.GetDonationsByUserIdAsync(user.Id);
            return View(donations ?? new List<Donation>());
        }

        private string GenerateReceiptNumber()
        {
            return $"GOTG-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}