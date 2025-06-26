using AutoMapper;
using Castle.Core.Internal;
using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;
using System.Threading.Tasks.Dataflow;

namespace Rapide.Services
{
    public class PaymentDetailsService(IPaymentDetailsRepo repo) : BaseService<PaymentDetails, PaymentDetailsDTO>(repo), IPaymentDetailsService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Payment, PaymentDTO>();
                cfg.CreateMap<PaymentDetails, PaymentDetailsDTO>();
                cfg.CreateMap<Parameter, ParameterDTO>();
                cfg.CreateMap<ParameterGroup, ParameterGroupDTO>();
                cfg.CreateMap<Invoice, InvoiceDTO>();

                cfg.CreateMap<InvoiceDTO, Invoice>();
                cfg.CreateMap<PaymentDTO, Payment>();
                cfg.CreateMap<PaymentDetailsDTO, PaymentDetails>();
                cfg.CreateMap<ParameterDTO, Parameter>();
                cfg.CreateMap<ParameterGroupDTO, ParameterGroup>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<PaymentDetailsDTO>> GetAllPaymentDetailsAsync()
        {
            try
            {
                List<PaymentDetailsDTO> dtoList = new List<PaymentDetailsDTO>();
                var entityList = await repo.GetAllPaymentDetailsAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<PaymentDetailsDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<PaymentDetailsDTO?> GetAsync(Expression<Func<PaymentDetails, bool>> predicate)
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

        public async Task<PaymentDetailsDTO?> GetPaymentDetailsByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetPaymentDetailsByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<PaymentDetailsDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<PaymentDetailsDTO>> GetAllPaymentDetailsByPaymentIdAsync(int paymentId)
        {
            try
            {
                List<PaymentDetailsDTO> dtoList = new List<PaymentDetailsDTO>();
                var entityList = await repo.GetAllPaymentDetailsByPaymentIdAsync(paymentId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<PaymentDetailsDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<PaymentDetailsDTO>> GetAllPaymentDetailsByInvoiceIdAsync(int invoiceId)
        {
            try
            {
                List<PaymentDetailsDTO> dtoList = new List<PaymentDetailsDTO>();
                var entityList = await repo.GetAllPaymentDetailsByInvoiceIdAsync(invoiceId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<PaymentDetailsDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<PaymentDetailsDTO> CreateAsync(PaymentDetailsDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Payment = null;
            dto.PaymentTypeParameter = null;
            dto.Invoice = null;

            var dtoMap = new Entities.PaymentDetails()
            {
                PaymentId = dto.PaymentId,
                PaymentTypeParameterId = dto.PaymentTypeParameterId,
                InvoiceId = dto.InvoiceId,
                //DepositAmount = dto.DepositAmount,
                AmountPaid = dto.AmountPaid,
                IsFullyPaid = dto.IsFullyPaid,
                IsDeposit = dto.IsDeposit,
                PaymentDate = dto.PaymentDate,
                PaymentReferenceNo = dto.PaymentReferenceNo,
                CreatedById = dto.CreatedById,
                CreatedDateTime = dto.CreatedDateTime,
                UpdatedById = dto.UpdatedById,
                UpdatedDateTime = dto.UpdatedDateTime
            };

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<PaymentDetailsDTO> UpdateAsync(PaymentDetailsDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Payment = null;
            dto.PaymentTypeParameter = null;
            dto.Invoice = null;

            var dtoMap = mapper.Map<Entities.PaymentDetails>(dto);

            var dataToCheck = await GetByIdAsync(dto.Id);

            if (dataToCheck == null)
            {
                dto.PaymentId = dto.PaymentId;
                dto.UpdatedById = dto.UpdatedById;
                dto.UpdatedDateTime = DateTime.Now;

                await CreateAsync(dto);
            }
            else
            {
                await base.UpdateByEntityAsync(dtoMap);
            }

            return dto;
        }
    }
}