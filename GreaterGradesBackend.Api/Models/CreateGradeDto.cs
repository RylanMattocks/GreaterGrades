using GreaterGradesBackend.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace GreaterGradesBackend.Api.Models
{
    public class CreateGradeDto
    {
        [Required]
        public int User { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        public int Score { get; set; }

        public GradeStatus GradingStatus { get; set; } = GradeStatus.NotGraded;
    }
}
