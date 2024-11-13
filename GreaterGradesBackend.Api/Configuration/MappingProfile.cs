using AutoMapper;
using GreaterGradesBackend.Domain.Entities;
using GreaterGradesBackend.Api.Models;
using GreaterGradesBackend.Services.Interfaces;
using GreaterGradesBackend.Services.Implementations;
using System.Collections.Generic;
using System.Linq;

namespace GreaterGradesBackend.Api.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            // Student to StudentDto mapping
            //CreateMap<Student, StudentDto>().ReverseMap();
            //CreateMap<CreateStudentDto, Student>();
            //CreateMap<UpdateStudentDto, Student>();

            // Class mappings
            CreateMap<Class, ClassDto>().ReverseMap();
            CreateMap<CreateClassDto, Class>();
            CreateMap<UpdateClassDto, Class>();

            // Assignment mappings
            CreateMap<Assignment, AssignmentDto>().ReverseMap();
            CreateMap<CreateAssignmentDto, Assignment>();
            CreateMap<UpdateAssignmentDto, Assignment>();

            // Grade mappings
            CreateMap<Grade, GradeDto>().ReverseMap();
            CreateMap<CreateGradeDto, Grade>();
            CreateMap<UpdateGradeDto, Grade>();

            // User mappings
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<LoginDto, User>();

            CreateMap<Institution, InstitutionDto>().ReverseMap();
            CreateMap<CreateInstitutionDto, Institution>();
            CreateMap<UpdateInstitutionDto, Institution>();

        }
    }
}
