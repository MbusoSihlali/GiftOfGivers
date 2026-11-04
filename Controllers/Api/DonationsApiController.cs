using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GiftOfGivers.Models;
using GiftOfGivers.Repositories;

namespace GiftOfGivers.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonationsApiController : ControllerBase
    {
        private readonly IDonationRepository _donationRepository;
        private readonly ILogger<DonationsApiController> _logger;

        public DonationsApiController(
            IDonationRepository donationRepository,
            ILogger<DonationsApiController> logger)
        {
            _donationRepository = donationRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDonations()
        {
            try
            {
                var donations = await _donationRepository.GetAllAsync();
                return Ok(donations.OrderByDescending(d => d.CreatedAt));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving donations");
                return StatusCode(500, "An error occurred while retrieving donations");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDonation(int id)
        {
            try
            {
                var donation = await _donationRepository.GetByIdAsync(id);
                if (donation == null)
                {
                    return NotFound($"Donation with ID {id} not found");
                }
                return Ok(donation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving donation {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the donation");
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserDonations(string userId)
        {
            try
            {
                var donations = await _donationRepository.GetDonationsByUserIdAsync(userId);
                return Ok(donations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving donations for user {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving user donations");
            }
        }

        [HttpGet("recent/{count}")]
        public async Task<IActionResult> GetRecentDonations(int count)
        {
            try
            {
                var donations = await _donationRepository.GetRecentDonationsAsync(count);
                return Ok(donations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent donations");
                return StatusCode(500, "An error occurred while retrieving recent donations");
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var totalDonations = await _donationRepository.GetTotalDonationsAsync();
                var allDonations = await _donationRepository.GetAllAsync();
                
                var statistics = new
                {
                    TotalAmount = totalDonations,
                    TotalCount = allDonations.Count(),
                    AverageDonation = allDonations.Any() ? allDonations.Average(d => d.Amount) : 0,
                    CurrencyBreakdown = allDonations.GroupBy(d => d.Currency)
                        .Select(g => new { Currency = g.Key, Count = g.Count(), Total = g.Sum(d => d.Amount) })
                };
                
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving donation statistics");
                return StatusCode(500, "An error occurred while retrieving statistics");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDonation([FromBody] Donation donation)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                donation.Status = "Pending";
                donation.ReceiptNumber = GenerateReceiptNumber();
                donation.CreatedAt = DateTime.UtcNow;

                await _donationRepository.AddAsync(donation);
                _logger.LogInformation("New donation created: {Amount} {Currency}", donation.Amount, donation.Currency);

                return CreatedAtAction(nameof(GetDonation), new { id = donation.Id }, donation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating donation");
                return StatusCode(500, "An error occurred while creating the donation");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateDonation(int id, [FromBody] Donation donation)
        {
            try
            {
                if (id != donation.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var existingDonation = await _donationRepository.GetByIdAsync(id);
                if (existingDonation == null)
                {
                    return NotFound($"Donation with ID {id} not found");
                }

                existingDonation.DonorName = donation.DonorName;
                existingDonation.DonorEmail = donation.DonorEmail;
                existingDonation.Amount = donation.Amount;
                existingDonation.Currency = donation.Currency;
                existingDonation.DonationType = donation.DonationType;
                existingDonation.Status = donation.Status;
                existingDonation.UpdatedAt = DateTime.UtcNow;

                await _donationRepository.UpdateAsync(existingDonation);
                _logger.LogInformation("Donation {Id} updated", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating donation {Id}", id);
                return StatusCode(500, "An error occurred while updating the donation");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDonation(int id)
        {
            try
            {
                var donation = await _donationRepository.GetByIdAsync(id);
                if (donation == null)
                {
                    return NotFound($"Donation with ID {id} not found");
                }

                await _donationRepository.DeleteAsync(donation);
                _logger.LogInformation("Donation {Id} deleted", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting donation {Id}", id);
                return StatusCode(500, "An error occurred while deleting the donation");
            }
        }

        private string GenerateReceiptNumber()
        {
            return $"GOTG-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}