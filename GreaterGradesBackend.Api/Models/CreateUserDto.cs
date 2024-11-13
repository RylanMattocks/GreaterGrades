using System.ComponentModel.DataAnnotations;
using GreaterGradesBackend.Domain.Enums;


namespace GreaterGradesBackend.Api.Models
{
    public class CreateUserDto
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        
        public Role Role { get; set; }

        [Required]
        public int InstitutionId { get; set; }
    }
}
