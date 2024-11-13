using GreaterGradesBackend.Api.Models;
using GreaterGradesBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstitutionsController : ControllerBase
    {
        private readonly IInstitutionService _institutionService;
        private readonly IUserService _userService;

        public InstitutionsController(IInstitutionService institutionService, IUserService userService)
        {
            _institutionService = institutionService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InstitutionDto>>> GetAllInstitutions()
        {
            var institutions = await _institutionService.GetAllInstitutionsAsync();
            return Ok(institutions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InstitutionDto>> GetInstitutionById(int id)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            if (currentUserRole == "InstitutionAdmin")
            {
                var user = await _userService.GetUserByIdAsync(currentUserId);
                if (user.InstitutionId != id)
                {
                    return Forbid();
                }
            }

            var institution = await _institutionService.GetInstitutionByIdAsync(id);
            if (institution == null)
            {
                return NotFound();
            }
            return Ok(institution);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<InstitutionDto>> CreateInstitution(CreateInstitutionDto createInstitutionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdInstitution = await _institutionService.CreateInstitutionAsync(createInstitutionDto);
            return CreatedAtAction(nameof(GetInstitutionById), new { id = createdInstitution.InstitutionId }, createdInstitution);
        }

        [Authorize(Roles = "Admin,InstitutionAdmin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInstitution(int id, UpdateInstitutionDto updateInstitutionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            if (currentUserRole == "InstitutionAdmin")
            {
                var user = await _userService.GetUserByIdAsync(currentUserId);
                if (user.InstitutionId != id)
                {
                    return Forbid();
                }
            }

            var result = await _institutionService.UpdateInstitutionAsync(id, updateInstitutionDto);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInstitution(int id)
        {
            var result = await _institutionService.DeleteInstitutionAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
