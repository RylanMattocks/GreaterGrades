using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreaterGradesBackend.Domain.Entities
{
    public class Institution
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InstitutionId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }


        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<Class> Classes { get; set; }

        [NotMapped]
        public IEnumerable<int> ClassIds => Classes.Select(c => c.ClassId);
        [NotMapped]
        public IEnumerable<int> UsersIds => Users.Select(u => u.UserId);

        public Institution()
        {
            Users = new HashSet<User>();
            Classes = new HashSet<Class>();
        }
    }
}
