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
    public class GradesController : ControllerBase
    {
        private readonly IGradeService _gradeService;
        private readonly IAssignmentService _assignmentService;
        private readonly IClassService _classService;
        private readonly IUserService _userService;

        public GradesController(IGradeService gradeService, IAssignmentService assignmentService, IClassService classService, IUserService userService)
        {
            _gradeService = gradeService;
            _assignmentService = assignmentService;
            _classService = classService;
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GradeDto>>> GetAllGrades()
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            if (currentUserRole == "InstitutionAdmin")
            {
                var user = await _userService.GetUserByIdAsync(currentUserId);
                var institutionGrades = await _gradeService.GetGradesByInstitutionIdAsync(user.InstitutionId);
                return Ok(institutionGrades);
            }

            var grades = await _gradeService.GetAllGradesAsync();
            return Ok(grades);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<GradeDto>> GetGradeById(int id)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var user = await _userService.GetUserByIdAsync(currentUserId);

            var grade = await _gradeService.GetGradeByIdAsync(id);
            if (grade == null)
            {
                return NotFound();
            }

            var assignment = await _assignmentService.GetAssignmentByIdAsync(grade.AssignmentId);
            var classDto = await _classService.GetClassByIdAsync(assignment.ClassId);

            if (currentUserRole == "Admin" ||
                (currentUserRole == "InstitutionAdmin" && classDto.InstitutionId == user.InstitutionId) ||
                (currentUserRole == "Teacher" && user.TaughtClassIds.Contains(assignment.ClassId)) ||
                (currentUserRole == "Student" && grade.UserId == currentUserId))
            {
                return Ok(grade);
            }

            return Forbid();
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGrade(int id, UpdateGradeDto updateGradeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var user = await _userService.GetUserByIdAsync(currentUserId);

            var grade = await _gradeService.GetGradeByIdAsync(id);
            if (grade == null)
            {
                return NotFound();
            }

            var assignment = await _assignmentService.GetAssignmentByIdAsync(grade.AssignmentId);
            var classDto = await _classService.GetClassByIdAsync(assignment.ClassId);

            if (currentUserRole == "Admin" ||
                (currentUserRole == "InstitutionAdmin" && classDto.InstitutionId == user.InstitutionId) ||
                (currentUserRole == "Teacher" && user.TaughtClassIds.Contains(assignment.ClassId)))
            {
                var result = await _gradeService.UpdateGradeAsync(id, updateGradeDto);
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
