using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GiftOfGivers.Models;
using GiftOfGivers.Repositories;

namespace GiftOfGivers.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class VolunteersApiController : ControllerBase
    {
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly ILogger<VolunteersApiController> _logger;

        public VolunteersApiController(
            IVolunteerRepository volunteerRepository,
            ILogger<VolunteersApiController> logger)
        {
            _volunteerRepository = volunteerRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVolunteers()
        {
            try
            {
                var volunteers = await _volunteerRepository.GetAllAsync();
                return Ok(volunteers.OrderByDescending(v => v.CreatedAt));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving volunteers");
                return StatusCode(500, "An error occurred while retrieving volunteers");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVolunteer(int id)
        {
            try
            {
                var volunteer = await _volunteerRepository.GetByIdAsync(id);
                if (volunteer == null)
                {
                    return NotFound($"Volunteer with ID {id} not found");
                }
                return Ok(volunteer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving volunteer {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the volunteer");
            }
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> GetVolunteersByStatus(string status)
        {
            try
            {
                var volunteers = await _volunteerRepository.GetVolunteersByStatusAsync(status);
                return Ok(volunteers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving volunteers with status {Status}", status);
                return StatusCode(500, "An error occurred while retrieving volunteers");
            }
        }

        [HttpGet("skill/{skill}")]
        public async Task<IActionResult> GetVolunteersBySkill(string skill)
        {
            try
            {
                var volunteers = await _volunteerRepository.GetVolunteersBySkillAsync(skill);
                return Ok(volunteers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving volunteers with skill {Skill}", skill);
                return StatusCode(500, "An error occurred while retrieving volunteers");
            }
        }

        [HttpGet("location/{city}/{province}")]
        public async Task<IActionResult> GetVolunteersByLocation(string city, string province)
        {
            try
            {
                var volunteers = await _volunteerRepository.GetVolunteersByLocationAsync(city, province);
                return Ok(volunteers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving volunteers in {City}, {Province}", city, province);
                return StatusCode(500, "An error occurred while retrieving volunteers");
            }
        }

        [HttpGet("statistics")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var allVolunteers = await _volunteerRepository.GetAllAsync();
                var pendingVolunteers = await _volunteerRepository.GetVolunteersByStatusAsync("Pending");
                var approvedVolunteers = await _volunteerRepository.GetVolunteersByStatusAsync("Approved");
                
                var statistics = new
                {
                    TotalCount = allVolunteers.Count(),
                    PendingCount = pendingVolunteers.Count(),
                    ApprovedCount = approvedVolunteers.Count(),
                    RejectedCount = allVolunteers.Count(v => v.Status == "Rejected"),
                    SkillBreakdown = allVolunteers.GroupBy(v => v.Skills)
                        .Select(g => new { Skill = g.Key, Count = g.Count() })
                        .OrderByDescending(g => g.Count),
                    LocationBreakdown = allVolunteers.GroupBy(v => new { v.City, v.Province })
                        .Select(g => new { Location = $"{g.Key.City}, {g.Key.Province}", Count = g.Count() })
                        .OrderByDescending(g => g.Count)
                };
                
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving volunteer statistics");
                return StatusCode(500, "An error occurred while retrieving statistics");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateVolunteer([FromBody] Volunteer volunteer)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                volunteer.Status = "Pending";
                volunteer.CreatedAt = DateTime.UtcNow;

                await _volunteerRepository.AddAsync(volunteer);
                _logger.LogInformation("New volunteer application submitted by {Name}", volunteer.FullName);

                return CreatedAtAction(nameof(GetVolunteer), new { id = volunteer.Id }, volunteer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating volunteer application");
                return StatusCode(500, "An error occurred while creating the volunteer application");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateVolunteer(int id, [FromBody] Volunteer volunteer)
        {
            try
            {
                if (id != volunteer.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var existingVolunteer = await _volunteerRepository.GetByIdAsync(id);
                if (existingVolunteer == null)
                {
                    return NotFound($"Volunteer with ID {id} not found");
                }

                existingVolunteer.FullName = volunteer.FullName;
                existingVolunteer.Email = volunteer.Email;
                existingVolunteer.PhoneNumber = volunteer.PhoneNumber;
                existingVolunteer.City = volunteer.City;
                existingVolunteer.Province = volunteer.Province;
                existingVolunteer.Skills = volunteer.Skills;
                existingVolunteer.Availability = volunteer.Availability;
                existingVolunteer.Motivation = volunteer.Motivation;
                existingVolunteer.Status = volunteer.Status;
                existingVolunteer.UpdatedAt = DateTime.UtcNow;

                await _volunteerRepository.UpdateAsync(existingVolunteer);
                _logger.LogInformation("Volunteer {Id} updated", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating volunteer {Id}", id);
                return StatusCode(500, "An error occurred while updating the volunteer");
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateVolunteerStatus(int id, [FromBody] StatusUpdateDto model)
        {
            try
            {
                var volunteer = await _volunteerRepository.GetByIdAsync(id);
                if (volunteer == null)
                {
                    return NotFound($"Volunteer with ID {id} not found");
                }

                volunteer.Status = model.Status;
                volunteer.UpdatedAt = DateTime.UtcNow;

                await _volunteerRepository.UpdateAsync(volunteer);
                _logger.LogInformation("Volunteer {Id} status updated to {Status}", id, model.Status);

                return Ok(new { message = "Status updated successfully", status = model.Status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating volunteer {Id} status", id);
                return StatusCode(500, "An error occurred while updating the volunteer status");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteVolunteer(int id)
        {
            try
            {
                var volunteer = await _volunteerRepository.GetByIdAsync(id);
                if (volunteer == null)
                {
                    return NotFound($"Volunteer with ID {id} not found");
                }

                await _volunteerRepository.DeleteAsync(volunteer);
                _logger.LogInformation("Volunteer {Id} deleted", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting volunteer {Id}", id);
                return StatusCode(500, "An error occurred while deleting the volunteer");
            }
        }
    }
}