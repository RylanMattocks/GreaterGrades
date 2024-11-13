using System.ComponentModel.DataAnnotations;

namespace GreaterGradesBackend.Api.Models
{
    public class CreateAssignmentDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int ClassId { get; set; }
        [Required]
        public int MaxScore { get; set; }
    }
}
