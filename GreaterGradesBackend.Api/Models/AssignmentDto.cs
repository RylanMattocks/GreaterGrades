using System.Collections.Generic;

namespace GreaterGradesBackend.Api.Models
{
    public class AssignmentDto
    {
        public int AssignmentId { get; set; }
        public string Name { get; set; }

        public int ClassId { get; set; }

        public ICollection<GradeDto> Grades { get; set; }

        public int MaxScore { get; set; }
    }
}
