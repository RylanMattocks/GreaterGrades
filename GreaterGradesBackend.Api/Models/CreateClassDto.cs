using System.ComponentModel.DataAnnotations;

namespace GreaterGradesBackend.Api.Models
{
    public class CreateClassDto
    {
        [Required]
        [MaxLength(150)]
        public string Subject { get; set; }

        [Required]
        public int InstitutionId { get; set; }
    }
}
