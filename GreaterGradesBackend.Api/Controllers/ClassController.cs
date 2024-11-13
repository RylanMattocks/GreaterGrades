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
    public class ClassesController : ControllerBase
    {
        private readonly IClassService _classService;
        private readonly IUserService _userService;

        public ClassesController(IClassService classService, IUserService userService)
        {
            _classService = classService;
            _userService = userService;
        }

        [Authorize(Roles = "Admin,InstitutionAdmin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassDto>>> GetAllClasses()
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            if (currentUserRole == "InstitutionAdmin")
            {
                var user = await _userService.GetUserByIdAsync(currentUserId);
                var institutionClasses = await _classService.GetClassesByInstitutionIdAsync(user.InstitutionId);
                return Ok(institutionClasses);
            }

            var classes = await _classService.GetAllClassesAsync();
            return Ok(classes);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ClassDto>> GetClassById(int id)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            if (currentUserRole == "InstitutionAdmin")
            {
                var user = await _userService.GetUserByIdAsync(currentUserId);
                var classDto = await _classService.GetClassByIdAsync(id);
                if (classDto == null || classDto.InstitutionId != user.InstitutionId)
                {
                    return Forbid();
                }
                else
                {
                    return Ok(classDto);
                }
            }
            else
            {
                var classDto = await _classService.GetClassByIdAsync(id);
                if (classDto == null)
                {
                    return NotFound();
                }
                return Ok(classDto);
            }

            return Forbid();
        }

        [Authorize(Roles = "Admin,InstitutionAdmin,Teacher")]
        [HttpPost]
        public async Task<ActionResult<ClassDto>> CreateClass(CreateClassDto createClassDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            if (currentUserRole == "InstitutionAdmin" || currentUserRole == "Teacher")
            {
                var user = await _userService.GetUserByIdAsync(currentUserId);
                if (user.InstitutionId != createClassDto.InstitutionId)
                {
                    return Forbid();
                }
            }

            var createdClass = await _classService.CreateClassAsync(createClassDto);
            return CreatedAtAction(nameof(GetClassById), new { id = createdClass.ClassId }, createdClass);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClass(int id, UpdateClassDto updateClassDto)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var user = await _userService.GetUserByIdAsync(currentUserId);

            if (currentUserRole == "Admin")
            {
                // Admin can update any class
                var result = await _classService.UpdateClassAsync(id, updateClassDto);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }

            var classDto = await _classService.GetClassByIdAsync(id);

            if (classDto == null)
            {
                return NotFound();
            }

            if (currentUserRole == "InstitutionAdmin")
            {
                if (classDto.InstitutionId == user.InstitutionId)
                {
                    var result = await _classService.UpdateClassAsync(id, updateClassDto);
                    if (!result)
                    {
                        return NotFound();
                    }
                    return NoContent();
                }
            }
            else if (currentUserRole == "Teacher" && user.TaughtClassIds.Contains(id))
            {
                // Teachers can only update classes they teach
                var result = await _classService.UpdateClassAsync(id, updateClassDto);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }

            return Forbid();
        }

        [Authorize(Roles = "Admin,InstitutionAdmin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);

            if (currentUserRole == "InstitutionAdmin")
            {
                var user = await _userService.GetUserByIdAsync(currentUserId);
                var classDto = await _classService.GetClassByIdAsync(id);
                if (classDto.InstitutionId != user.InstitutionId)
                {
                    return Forbid();
                }
            }

            var result = await _classService.DeleteClassAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [Authorize]
        [HttpPost("{id}/students/{studentId}")]
        public async Task<IActionResult> AddStudentToClass(int id, int studentId)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var user = await _userService.GetUserByIdAsync(currentUserId);

            if (currentUserRole == "Admin" || currentUserRole == "InstitutionAdmin" || currentUserRole == "Teacher")
            {
                var classDto = await _classService.GetClassByIdAsync(id);
                if (classDto == null || (!user.TaughtClassIds.Contains(id) && currentUserRole == "Teacher"))
                {
                    return Forbid();
                }

                var result = await _classService.AddStudentToClassAsync(id, studentId);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }

            return Forbid();
        }

        [Authorize]
        [HttpDelete("{id}/students/{studentId}")]
        public async Task<IActionResult> RemoveStudentFromClass(int id, int studentId)
        {
            Console.WriteLine("ITS DOING SOMTHING \n\n\n\n\n\n\n");
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var user = await _userService.GetUserByIdAsync(currentUserId);


            if (currentUserRole == "Admin" || currentUserRole == "InstitutionAdmin" || currentUserRole == "Teacher")
            {
                var classDto = await _classService.GetClassByIdAsync(id);
                if (classDto == null || (!user.TaughtClassIds.Contains(id) && currentUserRole == "Teacher"))
                {
                    return Forbid();
                }

                var result = await _classService.RemoveStudentFromClassAsync(id, studentId);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }

            return Forbid();
        }

        [Authorize]
        [HttpPost("{id}/teachers/{teacherId}")]
        public async Task<IActionResult> AddTeacherToClass(int id, int teacherId)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var user = await _userService.GetUserByIdAsync(currentUserId);

            if (currentUserRole == "Admin" || currentUserRole == "InstitutionAdmin" || currentUserRole == "Teacher")
            {
                var classDto = await _classService.GetClassByIdAsync(id);
                if (classDto == null)
                {
                    return Forbid();
                }

                var result = await _classService.AddTeacherToClassAsync(id, teacherId);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }

            return Forbid();
        }

        [Authorize]
        [HttpDelete("{id}/teachers/{teacherId}")]
        public async Task<IActionResult> RemoveTeacherFromClass(int id, int teacherId)
        {
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
            var user = await _userService.GetUserByIdAsync(currentUserId);


            if (currentUserRole == "Admin" || currentUserRole == "InstitutionAdmin" || currentUserRole == "Teacher")
            {
                var classDto = await _classService.GetClassByIdAsync(id);
                if (classDto == null || (!user.TaughtClassIds.Contains(id) && currentUserRole == "Teacher"))
                {
                    return Forbid();
                }

                var result = await _classService.RemoveTeacherFromClassAsync(id, teacherId);
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
