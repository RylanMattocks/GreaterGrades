using GreaterGradesBackend.Domain.Enums;

namespace GreaterGradesBackend.Api.Models
{
    public class UpdateGradeDto
    {
        public int Score { get; set; }

        public GradeStatus GradingStatus { get; set; }
    }
}
