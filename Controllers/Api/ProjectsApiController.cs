using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GiftOfGivers.Models;
using GiftOfGivers.Repositories;

namespace GiftOfGivers.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsApiController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ILogger<ProjectsApiController> _logger;

        public ProjectsApiController(
            IProjectRepository projectRepository,
            ILogger<ProjectsApiController> logger)
        {
            _projectRepository = projectRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            try
            {
                var projects = await _projectRepository.GetAllAsync();
                return Ok(projects.OrderByDescending(p => p.CreatedAt));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects");
                return StatusCode(500, "An error occurred while retrieving projects");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            try
            {
                var project = await _projectRepository.GetByIdAsync(id);
                if (project == null)
                {
                    return NotFound($"Project with ID {id} not found");
                }
                return Ok(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the project");
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveProjects()
        {
            try
            {
                var projects = await _projectRepository.GetActiveProjectsAsync();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active projects");
                return StatusCode(500, "An error occurred while retrieving active projects");
            }
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetProjectsByCategory(string category)
        {
            try
            {
                var projects = await _projectRepository.GetProjectsByCategoryAsync(category);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects in category {Category}", category);
                return StatusCode(500, "An error occurred while retrieving projects");
            }
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> GetProjectsByStatus(string status)
        {
            try
            {
                var projects = await _projectRepository.GetProjectsByStatusAsync(status);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects with status {Status}", status);
                return StatusCode(500, "An error occurred while retrieving projects");
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var allProjects = await _projectRepository.GetAllAsync();
                var activeProjects = await _projectRepository.GetActiveProjectsAsync();
                var totalRaised = await _projectRepository.GetTotalRaisedAmountAsync();
                
                var statistics = new
                {
                    TotalCount = allProjects.Count(),
                    ActiveCount = activeProjects.Count(),
                    CompletedCount = allProjects.Count(p => p.Status == "Completed"),
                    TotalRaised = totalRaised,
                    TotalTarget = allProjects.Sum(p => p.TargetAmount),
                    CategoryBreakdown = allProjects.GroupBy(p => p.Category)
                        .Select(g => new { Category = g.Key, Count = g.Count(), TotalTarget = g.Sum(p => p.TargetAmount) }),
                    AverageProgress = allProjects.Any() ? allProjects.Average(p => p.TargetAmount > 0 ? (p.RaisedAmount / p.TargetAmount) * 100 : 0) : 0
                };
                
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project statistics");
                return StatusCode(500, "An error occurred while retrieving statistics");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> CreateProject([FromBody] Project project)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                project.Status = "Active";
                project.RaisedAmount = 0;
                project.CreatedAt = DateTime.UtcNow;

                await _projectRepository.AddAsync(project);
                _logger.LogInformation("New project created: {Title}", project.Title);

                return CreatedAtAction(nameof(GetProject), new { id = project.Id }, project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                return StatusCode(500, "An error occurred while creating the project");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] Project project)
        {
            try
            {
                if (id != project.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var existingProject = await _projectRepository.GetByIdAsync(id);
                if (existingProject == null)
                {
                    return NotFound($"Project with ID {id} not found");
                }

                existingProject.Title = project.Title;
                existingProject.Category = project.Category;
                existingProject.Description = project.Description;
                existingProject.Location = project.Location;
                existingProject.TargetAmount = project.TargetAmount;
                existingProject.Status = project.Status;
                existingProject.StartDate = project.StartDate;
                existingProject.EndDate = project.EndDate;
                existingProject.ImageUrl = project.ImageUrl;
                existingProject.UpdatedAt = DateTime.UtcNow;

                await _projectRepository.UpdateAsync(existingProject);
                _logger.LogInformation("Project {Id} updated", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project {Id}", id);
                return StatusCode(500, "An error occurred while updating the project");
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Employee")]
        public async Task<IActionResult> UpdateProjectStatus(int id, [FromBody] StatusUpdateDto model)
        {
            try
            {
                var project = await _projectRepository.GetByIdAsync(id);
                if (project == null)
                {
                    return NotFound($"Project with ID {id} not found");
                }

                project.Status = model.Status;
                project.UpdatedAt = DateTime.UtcNow;

                await _projectRepository.UpdateAsync(project);
                _logger.LogInformation("Project {Id} status updated to {Status}", id, model.Status);

                return Ok(new { message = "Status updated successfully", status = model.Status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project {Id} status", id);
                return StatusCode(500, "An error occurred while updating the project status");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                var project = await _projectRepository.GetByIdAsync(id);
                if (project == null)
                {
                    return NotFound($"Project with ID {id} not found");
                }

                await _projectRepository.DeleteAsync(project);
                _logger.LogInformation("Project {Id} deleted", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project {Id}", id);
                return StatusCode(500, "An error occurred while deleting the project");
            }
        }
    }
}