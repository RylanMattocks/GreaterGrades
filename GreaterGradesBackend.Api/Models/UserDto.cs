using GreaterGradesBackend.Domain.Enums;
using GreaterGradesBackend.Domain.Entities;

namespace GreaterGradesBackend.Api.Models
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<int> ClassIds { get; set; }
        public ICollection<int> TaughtClassIds { get; set; }
        public ICollection<int> GradeIds { get; set; }
        public Role Role { get; set; }
        public int InstitutionId { get; set; }
    }
}
