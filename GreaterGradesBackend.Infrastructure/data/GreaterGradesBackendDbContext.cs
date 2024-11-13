using GreaterGradesBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GreaterGradesBackend.Domain.Enums;

namespace GreaterGradesBackend.Infrastructure
{
    public class GreaterGradesBackendDbContext : DbContext
    {

        private readonly IPasswordHasher<User> _passwordHasher;
        public GreaterGradesBackendDbContext(DbContextOptions<GreaterGradesBackendDbContext> options, IPasswordHasher<User> passwordHasher)
            : base(options)
        {
            _passwordHasher = passwordHasher;
        }

        public DbSet<Class> Classes { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Institution> Institutions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            /////////////////////////// SEEDING DATA ////////////////////////////////
        
            modelBuilder.Entity<Institution>().HasData(
                new Institution { InstitutionId = 1, Name = "Institution 1", Address = "1600 Pennsylvania Avenue NW, Washington, DC 20500"},
                new Institution { InstitutionId = 2, Name = "Institution 2", Address = "600 1st St W, Mt Vernon, IA 52314"}
            );

            modelBuilder.Entity<User>().HasData(
                new User {UserId = 1, Username = "student1", PasswordHash = _passwordHasher.HashPassword(null, "student1"), Role = Role.Student, FirstName = "student1", LastName = "student1", InstitutionId = 1},
                new User {UserId = 2, Username = "student2", PasswordHash = _passwordHasher.HashPassword(null, "student2"), Role = Role.Student, FirstName = "student2", LastName = "student2", InstitutionId = 2},
                new User {UserId = 3, Username = "studentTA1", PasswordHash = _passwordHasher.HashPassword(null, "studentTA1"), Role = Role.Student, FirstName = "studentTA1", LastName = "studentTA1", InstitutionId = 1},
                new User {UserId = 4, Username = "studentTA2", PasswordHash = _passwordHasher.HashPassword(null, "studentTA2"), Role = Role.Student, FirstName = "studentTA2", LastName = "studentTA2", InstitutionId = 2},

                new User {UserId = 5, Username = "teacher1", PasswordHash = _passwordHasher.HashPassword(null, "teacher1"), Role = Role.Teacher, FirstName = "teacher1", LastName = "teacher1", InstitutionId = 1},
                new User {UserId = 6, Username = "teacher2", PasswordHash = _passwordHasher.HashPassword(null, "teacher2"), Role = Role.Teacher, FirstName = "teacher2", LastName = "teacher2", InstitutionId = 2},
                new User {UserId = 7, Username = "teacherST1", PasswordHash = _passwordHasher.HashPassword(null, "teacherST1"), Role = Role.Teacher, FirstName = "teacherST1", LastName = "teacherST1", InstitutionId = 1},
                new User {UserId = 8, Username = "teacherST2", PasswordHash = _passwordHasher.HashPassword(null, "teacherST2"), Role = Role.Teacher, FirstName = "teacherST2", LastName = "teacherST2", InstitutionId = 2},
                
                new User {UserId = 9, Username = "iadmin1", PasswordHash = _passwordHasher.HashPassword(null, "iadmin1"), Role = Role.InstitutionAdmin, FirstName = "iadmin1", LastName = "iadmin1", InstitutionId = 1},
                new User {UserId = 10, Username = "iadmin2", PasswordHash = _passwordHasher.HashPassword(null, "iadmin2"), Role = Role.InstitutionAdmin, FirstName = "iadmin2", LastName = "iadmin2", InstitutionId = 2},

                new User {UserId = 11, Username = "admin1", PasswordHash = _passwordHasher.HashPassword(null, "admin1"), Role = Role.Admin, FirstName = "admin1", LastName = "admin1", InstitutionId = 1},
                new User {UserId = 12, Username = "admin2", PasswordHash = _passwordHasher.HashPassword(null, "admin2"), Role = Role.Admin, FirstName = "admin2", LastName = "admin2", InstitutionId = 2}
            );

            modelBuilder.Entity<Class>().HasData(
                new Class {ClassId = 1, Subject = "math1", InstitutionId = 1},
                new Class {ClassId = 2, Subject = "math2", InstitutionId = 2},

                new Class {ClassId = 3, Subject = "english1", InstitutionId = 1},
                new Class {ClassId = 4, Subject = "english2", InstitutionId = 2},

                new Class {ClassId = 5, Subject = "science1", InstitutionId = 1},
                new Class {ClassId = 6, Subject = "science2", InstitutionId = 2},

                new Class {ClassId = 7, Subject = "history1", InstitutionId = 1},
                new Class {ClassId = 8, Subject = "history2", InstitutionId = 2}
            );
            
            modelBuilder.Entity<Assignment>().HasData(
                new Assignment {AssignmentId = 1, Name = "assignment1", ClassId = 1, MaxScore = 100},
                new Assignment {AssignmentId = 2, Name = "assignment2", ClassId = 2, MaxScore = 100},
                new Assignment {AssignmentId = 3, Name = "assignment3", ClassId = 3, MaxScore = 100},
                new Assignment {AssignmentId = 4, Name = "assignment4", ClassId = 4, MaxScore = 100},
                new Assignment {AssignmentId = 5, Name = "assignment5", ClassId = 5, MaxScore = 100},
                new Assignment {AssignmentId = 6, Name = "assignment6", ClassId = 6, MaxScore = 100},
                new Assignment {AssignmentId = 7, Name = "assignment7", ClassId = 7, MaxScore = 100},
                new Assignment {AssignmentId = 8, Name = "assignment8", ClassId = 8, MaxScore = 100}
            );

            modelBuilder.Entity<Grade>().HasData(
                new Grade {GradeId = 1, UserId = 1, AssignmentId = 1, Score = 20, GradingStatus = GradeStatus.Graded},
                new Grade {GradeId = 2, UserId = 2, AssignmentId = 2, Score = 30, GradingStatus = GradeStatus.Graded},
                new Grade {GradeId = 3, UserId = 3, AssignmentId = 3, Score = 40, GradingStatus = GradeStatus.Graded},
                new Grade {GradeId = 4, UserId = 4, AssignmentId = 4, Score = 50, GradingStatus = GradeStatus.Graded},
                new Grade {GradeId = 5, UserId = 7, AssignmentId = 5, Score = 60, GradingStatus = GradeStatus.Graded},
                new Grade {GradeId = 6, UserId = 8, AssignmentId = 6, Score = 70, GradingStatus = GradeStatus.Graded}
            );




            // Many-to-Many Relationship Configuration with Restrict for join tables
            modelBuilder.Entity<User>()
                .HasMany(s => s.Classes)
                .WithMany(c => c.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentClass",
                    sc => sc.HasOne<Class>().WithMany().HasForeignKey("ClassId").OnDelete(DeleteBehavior.Restrict),
                    sc => sc.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Restrict)
                ).HasData(
                    new { ClassId = 1, UserId = 1 },
                    new { ClassId = 2, UserId = 2 },
                    new { ClassId = 3, UserId = 3 },
                    new { ClassId = 4, UserId = 4 },
                    new { ClassId = 5, UserId = 7 },
                    new { ClassId = 6, UserId = 8 }
                );

            modelBuilder.Entity<User>()
                .HasMany(s => s.TaughtClasses)
                .WithMany(c => c.Teachers)
                .UsingEntity<Dictionary<string, object>>(
                    "TeacherClass",
                    sc => sc.HasOne<Class>().WithMany().HasForeignKey("ClassId").OnDelete(DeleteBehavior.Restrict),
                    sc => sc.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Restrict)
                ).HasData(
                    new { ClassId = 1, UserId = 3},
                    new { ClassId = 2, UserId = 4},
                    new { ClassId = 3, UserId = 5},
                    new { ClassId = 4, UserId = 6},
                    new { ClassId = 5, UserId = 7},
                    new { ClassId = 6, UserId = 8}
                );

            // Configure delete behavior for Grades
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.User)
                .WithMany(u => u.Grades)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Assignment)
                .WithMany(a => a.Grades)
                .HasForeignKey(g => g.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Institution cascade configuration
            modelBuilder.Entity<User>()
                .HasOne(u => u.Institution)
                .WithMany(i => i.Users)
                .HasForeignKey(u => u.InstitutionId) // Explicitly set InstitutionId as the foreign key
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Institution)
                .WithMany(i => i.Classes)
                .HasForeignKey(c => c.InstitutionId) // Explicitly set InstitutionId as the foreign key
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
