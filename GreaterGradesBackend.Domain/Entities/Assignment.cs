using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreaterGradesBackend.Domain.Entities
{
    public class Assignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssignmentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        // Relationship to Class
        [Required]
        public int ClassId { get; set; }

        [Required]
        public int MaxScore { get; set; }
        public Class Class { get; set; }  // Navigation property

        public ICollection<Grade> Grades { get; set; }

        public Assignment()
        {
            Grades = new HashSet<Grade>();
        }
    }
}
