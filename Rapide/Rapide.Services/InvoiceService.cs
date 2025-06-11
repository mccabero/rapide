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
    public class InvoiceService(IInvoiceRepo repo) : BaseService<Invoice, InvoiceDTO>(repo), IInvoiceService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Invoice, InvoiceDTO>();
                cfg.CreateMap<JobOrder, JobOrderDTO>();
                cfg.CreateMap<JobStatus, JobStatusDTO>();
                cfg.CreateMap<Customer, CustomerDTO>();
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Role, RoleDTO>();
                cfg.CreateMap<Vehicle, VehicleDTO>();
                cfg.CreateMap<VehicleModel, VehicleModelDTO>();
                cfg.CreateMap<VehicleMake, VehicleMakeDTO>();
                cfg.CreateMap<Parameter, ParameterDTO>();
                cfg.CreateMap<Estimate, EstimateDTO>();
                cfg.CreateMap<ServiceGroup, ServiceGroupDTO>();

                cfg.CreateMap<EstimateDTO, Estimate>();
                cfg.CreateMap<ParameterDTO, Parameter>();
                cfg.CreateMap<RoleDTO, Role>();
                cfg.CreateMap<VehicleDTO, Vehicle>();
                cfg.CreateMap<VehicleModelDTO, VehicleModel>();
                cfg.CreateMap<VehicleMakeDTO, VehicleMake>();
                cfg.CreateMap<JobOrderDTO, JobOrder>();
                cfg.CreateMap<InvoiceDTO, Invoice>();
                cfg.CreateMap<JobStatusDTO, JobStatus>();
                cfg.CreateMap<CustomerDTO, Customer>();
                cfg.CreateMap<UserDTO, User>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<InvoiceDTO>> GetAllInvoiceAsync()
        {
            try
            {
                List<InvoiceDTO> dtoList = new List<InvoiceDTO>();
                var entityList = await repo.GetAllInvoiceAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<InvoiceDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<InvoiceDTO>> GetAllInvoiceByCustomerIdAsync(int customerId)
        {
            try
            {
                List<InvoiceDTO> dtoList = new List<InvoiceDTO>();
                var entityList = await repo.GetAllInvoiceByCustomerIdAsync(customerId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<InvoiceDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<InvoiceDTO?> GetAsync(Expression<Func<Invoice, bool>> predicate)
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

        public async Task<InvoiceDTO?> GetInvoiceByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetInvoiceByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<InvoiceDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<InvoiceDTO> CreateAsync(InvoiceDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.JobStatus = null;
            dto.Customer = null;
            dto.AdvisorUser = null;
            dto.JobOrder = null;

            var createdDto = await base.CreateAsync(dto);

            return createdDto;
        }

        public override async Task<InvoiceDTO> UpdateAsync(InvoiceDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.JobStatus = null;
            dto.Customer = null;
            dto.AdvisorUser = null;
            dto.JobOrder = null;

            var dtoMap = mapper.Map<Invoice>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }
    }
}