using AutoMapper;
using Castle.Core.Internal;
using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Services
{
    public class InspectionService(IInspectionRepo repo) : BaseService<Inspection, InspectionDTO>(repo), IInspectionService
    {
        public async Task<List<InspectionDTO>> GetAllInspectionSummaryAsync()
        {
            try
            {
                List<InspectionDTO> dtoList = new List<InspectionDTO>();
                var entityList = await repo.GetAllInspectionSummaryAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = MappingHelper.InitializeMapper();
                dtoList = mapper.Map<List<InspectionDTO>>(entityList);

                //foreach (var e in entityList)
                //    dtoList.Add(mapper.Map<InspectionDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<InspectionDTO>> GetAllInspectionAsync()
        {
            try
            {
                List<InspectionDTO> dtoList = new List<InspectionDTO>();
                var entityList = await repo.GetAllInspectionAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = MappingHelper.InitializeMapper();
                dtoList = mapper.Map<List<InspectionDTO>>(entityList);

                //foreach (var e in entityList)
                //    dtoList.Add(mapper.Map<InspectionDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<InspectionDTO?> GetAsync(Expression<Func<Inspection, bool>> predicate)
        {
            try
            {
                var dto = await base.GetAsync(predicate);

                if (dto == null)
                    return null;

                return dto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<InspectionDTO?> GetInspectionByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetInspectionByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = MappingHelper.InitializeMapper();

                return mapper.Map<InspectionDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<InspectionDTO> CreateAsync(InspectionDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = MappingHelper.InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.JobStatus = null;
            dto.Customer = null;
            dto.Vehicle = null;
            dto.InspectorUser = null;
            dto.ServiceGroup = null;
            dto.AdvisorUser = null;
            dto.ApproverUser = null;
            dto.EstimatorUser = null;

            var createdDto = await base.CreateAsync(dto);

            return createdDto;
        }

        public override async Task<InspectionDTO> UpdateAsync(InspectionDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = MappingHelper.InitializeMapper();

            // Remove FKs. only parent table is to be inserted
            dto.JobStatus = null;
            dto.Customer = null;
            dto.Vehicle = null;
            dto.InspectorUser = null;
            dto.ServiceGroup = null;
            dto.AdvisorUser = null;
            dto.ApproverUser = null;
            dto.EstimatorUser = null;

            var dtoMap = mapper.Map<Inspection>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }

        public async Task<List<InspectionDTO>> GetAllInspectionByCustomerIdAsync(int customerId)
        {
            try
            {
                List<InspectionDTO> dtoList = new List<InspectionDTO>();
                var entityList = await repo.GetAllInspectionByCustomerIdAsync(customerId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = MappingHelper.InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<InspectionDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<InspectionDTO>> GetAllInspectionByVehicleIdAsync(int vehicleId)
        {
            try
            {
                List<InspectionDTO> dtoList = new List<InspectionDTO>();
                var entityList = await repo.GetAllInspectionByVehicleIdAsync(vehicleId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = MappingHelper.InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<InspectionDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}