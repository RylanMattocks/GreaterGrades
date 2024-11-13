using GreaterGradesBackend.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Services.Interfaces
{
    public interface IAssignmentService
    {
        Task<IEnumerable<AssignmentDto>> GetAllAssignmentsAsync();
        Task<AssignmentDto> GetAssignmentByIdAsync(int assignmentId);
        Task<AssignmentDto> CreateAssignmentAsync(CreateAssignmentDto createAssignmentDto);
        Task<bool> UpdateAssignmentAsync(int assignmentId, UpdateAssignmentDto updateAssignmentDto);
        Task<bool> DeleteAssignmentAsync(int assignmentId);
    }
}
