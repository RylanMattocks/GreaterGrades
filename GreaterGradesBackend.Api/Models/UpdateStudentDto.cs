using System.ComponentModel.DataAnnotations;

namespace GreaterGradesBackend.Api.Models
{
    public class UpdateStudentDto
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
    }
}
