using System.Collections.Generic;

namespace GreaterGradesBackend.Api.Models
{
    public class StudentDto
    {
        public int StudentId { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        // Only show Class IDs
        public ICollection<int> ClassIds { get; set; }
        
        // Only show Grade IDs
        public ICollection<int> GradeIds { get; set; }
    }
}
