using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Server.Models.Entities;
using NewsAggregation.Server.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IExternalServerService _externalServerService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IExternalServerService externalServerService,
            ICategoryService categoryService,
            ILogger<AdminController> logger)
        {
            _externalServerService = externalServerService;
            _categoryService = categoryService;
            _logger = logger;
        }

        // GET: api/Admin/servers
        [HttpGet("servers")]
        public async Task<IActionResult> GetAllServers()
        {
            try
            {
                var servers = await _externalServerService.GetAllServersAsync();
                
                var serverList = servers.Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.ServerType,
                    Status = s.IsActive ? "Active" : "Not Active",
                    LastAccessed = s.LastAccessed?.ToString("dd MMM yyyy") ?? "Never",
                    s.ApiUrl,
                    s.RequestsPerHour,
                    s.CurrentHourRequests,
                    s.CreatedAt
                }).ToList();

                return Ok(new
                {
                    Message = "External servers retrieved successfully",
                    Count = serverList.Count,
                    Servers = serverList
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving external servers");
                return StatusCode(500, new { Error = "An error occurred while retrieving servers" });
            }
        }

        // GET: api/Admin/servers/{id}
        [HttpGet("servers/{id:int}")]
        public async Task<IActionResult> GetServerById(int id)
        {
            try
            {
                var server = await _externalServerService.GetServerByIdAsync(id);
                if (server == null)
                    return NotFound(new { Message = "Server not found" });

                var serverDetails = new
                {
                    server.Id,
                    server.Name,
                    server.ServerType,
                    server.ApiUrl,
                    ApiKey = "***" + server.ApiKey.Substring(Math.Max(0, server.ApiKey.Length - 4)), // Mask API key
                    server.IsActive,
                    server.LastAccessed,
                    server.RequestsPerHour,
                    server.CurrentHourRequests,
                    server.LastHourReset,
                    server.CreatedAt
                };

                return Ok(new
                {
                    Message = "Server details retrieved successfully",
                    Server = serverDetails
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving server details for ID: {Id}", id);
                return StatusCode(500, new { Error = "An error occurred while retrieving server details" });
            }
        }

        // PUT: api/Admin/servers/{id}
        [HttpPut("servers/{id:int}")]
        public async Task<IActionResult> UpdateServer(int id, [FromBody] UpdateServerDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingServer = await _externalServerService.GetServerByIdAsync(id);
                if (existingServer == null)
                    return NotFound(new { Message = "Server not found" });

                // Update only the fields provided
                existingServer.Name = updateDto.Name ?? existingServer.Name;
                existingServer.ApiUrl = updateDto.ApiUrl ?? existingServer.ApiUrl;
                existingServer.ApiKey = updateDto.ApiKey ?? existingServer.ApiKey;
                existingServer.ServerType = updateDto.ServerType ?? existingServer.ServerType;
                existingServer.IsActive = updateDto.IsActive ?? existingServer.IsActive;
                existingServer.RequestsPerHour = updateDto.RequestsPerHour ?? existingServer.RequestsPerHour;

                var updatedServer = await _externalServerService.UpdateServerAsync(existingServer);

                return Ok(new
                {
                    Message = "Server updated successfully",
                    Server = new
                    {
                        updatedServer.Id,
                        updatedServer.Name,
                        updatedServer.ServerType,
                        updatedServer.ApiUrl,
                        updatedServer.IsActive,
                        updatedServer.RequestsPerHour
                    }
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating server with ID: {Id}", id);
                return StatusCode(500, new { Error = "An error occurred while updating server" });
            }
        }

        // POST: api/Admin/servers
        [HttpPost("servers")]
        public async Task<IActionResult> CreateServer([FromBody] CreateServerDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var server = new ExternalServer
                {
                    Name = createDto.Name,
                    ApiUrl = createDto.ApiUrl,
                    ApiKey = createDto.ApiKey,
                    ServerType = createDto.ServerType,
                    IsActive = createDto.IsActive,
                    RequestsPerHour = createDto.RequestsPerHour
                };

                var createdServer = await _externalServerService.CreateServerAsync(server);

                return CreatedAtAction(nameof(GetServerById), new { id = createdServer.Id }, new
                {
                    Message = "Server created successfully",
                    Server = new
                    {
                        createdServer.Id,
                        createdServer.Name,
                        createdServer.ServerType,
                        createdServer.ApiUrl,
                        createdServer.IsActive,
                        createdServer.RequestsPerHour
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new server");
                return StatusCode(500, new { Error = "An error occurred while creating server" });
            }
        }

        // DELETE: api/Admin/servers/{id}
        [HttpDelete("servers/{id:int}")]
        public async Task<IActionResult> DeleteServer(int id)
        {
            try
            {
                var result = await _externalServerService.DeleteServerAsync(id);
                if (!result)
                    return NotFound(new { Message = "Server not found" });

                return Ok(new { Message = "Server deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting server with ID: {Id}", id);
                return StatusCode(500, new { Error = "An error occurred while deleting server" });
            }
        }

        // PATCH: api/Admin/servers/{id}/toggle-status
        [HttpPatch("servers/{id:int}/toggle-status")]
        public async Task<IActionResult> ToggleServerStatus(int id)
        {
            try
            {
                var result = await _externalServerService.ToggleServerStatusAsync(id);
                if (!result)
                    return NotFound(new { Message = "Server not found" });

                var server = await _externalServerService.GetServerByIdAsync(id);
                return Ok(new
                {
                    Message = $"Server status toggled successfully. New status: {(server?.IsActive == true ? "Active" : "Inactive")}",
                    IsActive = server?.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling server status for ID: {Id}", id);
                return StatusCode(500, new { Error = "An error occurred while toggling server status" });
            }
        }

        // POST: api/Admin/categories
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var category = new Category
                {
                    Name = createDto.Name,
                    Description = createDto.Description,
                    IsActive = true
                };

                var createdCategory = await _categoryService.CreateCategoryAsync(category);

                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, new
                {
                    Message = "Category created successfully",
                    Category = new
                    {
                        createdCategory.Id,
                        createdCategory.Name,
                        createdCategory.Description,
                        createdCategory.IsActive,
                        createdCategory.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new category");
                return StatusCode(500, new { Error = "An error occurred while creating category" });
            }
        }

        // GET: api/Admin/categories/{id}
        [HttpGet("categories/{id:int}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                    return NotFound(new { Message = "Category not found" });

                return Ok(new
                {
                    Message = "Category retrieved successfully",
                    Category = new
                    {
                        category.Id,
                        category.Name,
                        category.Description,
                        category.IsActive,
                        category.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving category with ID: {Id}", id);
                return StatusCode(500, new { Error = "An error occurred while retrieving category" });
            }
        }
    }

    // DTOs for Admin operations
    public class UpdateServerDto
    {
        public string? Name { get; set; }
        public string? ApiUrl { get; set; }
        public string? ApiKey { get; set; }
        public string? ServerType { get; set; }
        public bool? IsActive { get; set; }
        public int? RequestsPerHour { get; set; }
    }

    public class CreateServerDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [Url]
        public string ApiUrl { get; set; } = string.Empty;
        
        [Required]
        public string ApiKey { get; set; } = string.Empty;
        
        [Required]
        public string ServerType { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        public int RequestsPerHour { get; set; } = 1000;
    }

    public class CreateCategoryDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(255)]
        public string? Description { get; set; }
    }
} 