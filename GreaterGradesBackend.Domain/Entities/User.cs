using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GreaterGradesBackend.Domain.Enums;


namespace GreaterGradesBackend.Domain.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }



        //TODO: Specify Unique
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public Role Role { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        public ICollection<Class> Classes { get; set; }
        public ICollection<Grade> Grades { get; set; }
        public ICollection<Class> TaughtClasses { get; set; }

        [ForeignKey("Institution")]
        public int InstitutionId { get; set; }
        
        public Institution Institution { get; set; } // Navigation property

        [NotMapped] 
        public IEnumerable<int> ClassIds => Classes.Select(c => c.ClassId);

        [NotMapped]
        public IEnumerable<int> TaughtClassIds => TaughtClasses.Select(c => c.ClassId);
        
        [NotMapped] 
        public IEnumerable<int> GradeIds => Grades.Select(g => g.GradeId);

        public User()
        {
            Role = Role.Student; // Default role
            TaughtClasses = new HashSet<Class>();
            Classes = new HashSet<Class>();
            Grades = new HashSet<Grade>();
        }
    }
}
