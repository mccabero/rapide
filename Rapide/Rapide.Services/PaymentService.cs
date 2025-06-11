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
    public class PaymentService(IPaymentRepo repo) : BaseService<Payment, PaymentDTO>(repo), IPaymentService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Payment, PaymentDTO>();
                cfg.CreateMap<JobStatus, JobStatusDTO>();
                cfg.CreateMap<Customer, CustomerDTO>();
                cfg.CreateMap<Parameter, ParameterDTO>();
                cfg.CreateMap<ParameterGroup, ParameterGroupDTO>();
                cfg.CreateMap<Invoice, InvoiceDTO>();

                cfg.CreateMap<InvoiceDTO, Invoice>();
                cfg.CreateMap<ParameterDTO, Parameter>();
                cfg.CreateMap<ParameterGroupDTO, ParameterGroup>();
                cfg.CreateMap<PaymentDTO, Payment>();
                cfg.CreateMap<JobStatusDTO, JobStatus>();
                cfg.CreateMap<CustomerDTO, Customer>();
                cfg.CreateMap<ServiceGroupDTO, ServiceGroup>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<PaymentDTO>> GetAllPaymentAsync()
        {
            try
            {
                List<PaymentDTO> dtoList = new List<PaymentDTO>();
                var entityList = await repo.GetAllPaymentAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<PaymentDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<PaymentDTO?> GetAsync(Expression<Func<Payment, bool>> predicate)
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

        public async Task<PaymentDTO?> GetPaymentByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetPaymentByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<PaymentDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<PaymentDTO> CreateAsync(PaymentDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.JobStatus = null;
            dto.Customer = null;

            var createdDto = await base.CreateAsync(dto);

            return createdDto;
        }

        public override async Task<PaymentDTO> UpdateAsync(PaymentDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.JobStatus = null;
            dto.Customer = null;

            var dtoMap = mapper.Map<Payment>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }

        public async Task<List<PaymentDTO>> GetAllPaymentByCustomerIdAsync(int customerId)
        {
            try
            {
                List<PaymentDTO> dtoList = new List<PaymentDTO>();
                var entityList = await repo.GetAllPaymentByCustomerIdAsync(customerId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<PaymentDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}