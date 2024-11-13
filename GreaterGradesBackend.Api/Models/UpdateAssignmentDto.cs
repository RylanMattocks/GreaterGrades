using System.ComponentModel.DataAnnotations;

namespace GreaterGradesBackend.Api.Models
{
    public class UpdateAssignmentDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int ClassId { get; set; }  // Update the class this assignment belongs to
    }
}
