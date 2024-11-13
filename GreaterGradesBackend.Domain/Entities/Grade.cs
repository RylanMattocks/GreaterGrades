using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GreaterGradesBackend.Domain.Enums;

namespace GreaterGradesBackend.Domain.Entities
{
    public class Grade
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GradeId { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; }

        public int Score { get; set; }
        public GradeStatus GradingStatus { get; set; }

        public Grade()
        {
            // When Grade is created, it is set to 'NotGraded'
            GradingStatus = GradeStatus.NotGraded;
        }
    }
}
