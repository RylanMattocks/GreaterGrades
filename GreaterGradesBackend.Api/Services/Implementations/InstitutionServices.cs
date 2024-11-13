using AutoMapper;
using GreaterGradesBackend.Api.Models;
using GreaterGradesBackend.Domain.Entities;
using GreaterGradesBackend.Domain.Interfaces;
using GreaterGradesBackend.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Services.Implementations
{
    public class InstitutionService : IInstitutionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InstitutionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Retrieve a specific Institution by its ID
        public async Task<InstitutionDto> GetInstitutionByIdAsync(int institutionId)
        {
            var institution = await _unitOfWork.Institutions.GetInstitutionWithDetailsAsync(institutionId);
            if (institution == null)
            {
                return null;
            }
            return _mapper.Map<InstitutionDto>(institution);
        }

        // Retrieve all Institutions
        public async Task<IEnumerable<InstitutionDto>> GetAllInstitutionsAsync()
        {
            var institutions = await _unitOfWork.Institutions.GetAllAsync();
            return _mapper.Map<IEnumerable<InstitutionDto>>(institutions);
        }

        // Create a new Institution
        public async Task<InstitutionDto> CreateInstitutionAsync(CreateInstitutionDto createInstitutionDto)
        {
            var institution = _mapper.Map<Institution>(createInstitutionDto);
            await _unitOfWork.Institutions.AddAsync(institution);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<InstitutionDto>(institution);
        }

        // Update an existing Institution
        public async Task<bool> UpdateInstitutionAsync(int institutionId, UpdateInstitutionDto updateInstitutionDto)
        {
            var institution = await _unitOfWork.Institutions.GetByIdAsync(institutionId);
            if (institution == null)
            {
                return false;
            }

            _mapper.Map(updateInstitutionDto, institution);
            _unitOfWork.Institutions.Update(institution);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        // Delete an Institution by its ID
        public async Task<bool> DeleteInstitutionAsync(int institutionId)
        {
            var institution = await _unitOfWork.Institutions.GetByIdAsync(institutionId);
            if (institution == null)
            {
                return false;
            }

            _unitOfWork.Institutions.Remove(institution);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
