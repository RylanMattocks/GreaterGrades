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
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;
        private readonly IClassService _classService;
        private readonly IUserService _userService;

        public AssignmentsController(IAssignmentService assignmentService, IClassService classService, IUserService userService)
        {
            _assignmentService = assignmentService;
            _classService = classService;
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetAllAssignments()
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            var assignments = await _assignmentService.GetAllAssignmentsAsync();
            return Ok(assignments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AssignmentDto>> GetAssignmentById(int id)
        {
            var assignment = await _assignmentService.GetAssignmentByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            return Ok(assignment);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<AssignmentDto>> CreateAssignment(CreateAssignmentDto createAssignmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var user = await _userService.GetUserByIdAsync(currentUserId);

            var classDto = await _classService.GetClassByIdAsync(createAssignmentDto.ClassId);

            if (currentUserRole == "Admin" ||
                (currentUserRole == "InstitutionAdmin" && classDto.InstitutionId == user.InstitutionId) ||
                user.TaughtClassIds.Contains(createAssignmentDto.ClassId))
            {
                var createdAssignment = await _assignmentService.CreateAssignmentAsync(createAssignmentDto);
                return CreatedAtAction(nameof(GetAssignmentById), new { id = createdAssignment.AssignmentId }, createdAssignment);
            }

            return Forbid();
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssignment(int id, UpdateAssignmentDto updateAssignmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var user = await _userService.GetUserByIdAsync(currentUserId);

            var assignment = await _assignmentService.GetAssignmentByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            var classDto = await _classService.GetClassByIdAsync(assignment.ClassId);

            if (currentUserRole == "Admin" ||
                (currentUserRole == "InstitutionAdmin" && classDto.InstitutionId == user.InstitutionId) ||
                user.TaughtClassIds.Contains(assignment.ClassId))
            {
                var result = await _assignmentService.UpdateAssignmentAsync(id, updateAssignmentDto);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }

            return Forbid();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var user = await _userService.GetUserByIdAsync(currentUserId);

            var assignment = await _assignmentService.GetAssignmentByIdAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            var classDto = await _classService.GetClassByIdAsync(assignment.ClassId);

            if (currentUserRole == "Admin" ||
                (currentUserRole == "InstitutionAdmin" && classDto.InstitutionId == user.InstitutionId) ||
                user.TaughtClassIds.Contains(assignment.ClassId))
            {
                var result = await _assignmentService.DeleteAssignmentAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }

            return Forbid();
        }
    }
}
