namespace GreaterGradesBackend.Api.Models
{
    public class GradeDto
    {
        public int GradeId { get; set; }

        public int UserId { get; set; }

        public int AssignmentId { get; set; }

        public int Score { get; set; }

        public string GradingStatus { get; set; }
    }
}
