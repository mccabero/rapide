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
    public class InvoicePackageService(IInvoicePackageRepo repo) : BaseService<InvoicePackage, InvoicePackageDTO>(repo), IInvoicePackageService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<InvoicePackage, InvoicePackageDTO>();
                cfg.CreateMap<Invoice, InvoiceDTO>();
                cfg.CreateMap<Package, PackageDTO>();

                cfg.CreateMap<PaymentDTO, Payment>();
                cfg.CreateMap<InvoicePackageDTO, InvoicePackage>();
                cfg.CreateMap<InvoiceDTO, Invoice>();
                cfg.CreateMap<PackageDTO, Package>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<InvoicePackageDTO>> GetAllInvoicePackageAsync()
        {
            try
            {
                List<InvoicePackageDTO> dtoList = new List<InvoicePackageDTO>();
                var entityList = await repo.GetAllInvoicePackageAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<InvoicePackageDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<InvoicePackageDTO?> GetAsync(Expression<Func<InvoicePackage, bool>> predicate)
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

        public async Task<InvoicePackageDTO?> GetInvoicePackageByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetInvoicePackageByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<InvoicePackageDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<InvoicePackageDTO>> GetAllInvoicePackageByInvoiceIdAsync(int invoiceId)
        {
            try
            {
                List<InvoicePackageDTO> dtoList = new List<InvoicePackageDTO>();
                var entityList = await repo.GetAllInvoicePackageByInvoiceIdAsync(invoiceId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<InvoicePackageDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<InvoicePackageDTO> CreateAsync(InvoicePackageDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Invoice = null;
            dto.Package = null;

            var dtoMap = new Entities.InvoicePackage()
            {
                InvoiceId = dto.InvoiceId,
                PackageId = dto.PackageId,
                IncentiveSA = dto.IncentiveSA,
                IncentiveTech = dto.IncentiveTech,
                CreatedById = dto.CreatedById,
                CreatedDateTime = dto.CreatedDateTime,
                UpdatedById = dto.UpdatedById,
                UpdatedDateTime = dto.UpdatedDateTime
            };

            await base.CreateByEntityAsync(dtoMap);

            return dto;
        }

        public override async Task<InvoicePackageDTO> UpdateAsync(InvoicePackageDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.Invoice = null;
            dto.Package = null;

            var dtoMap = mapper.Map<Entities.InvoicePackage>(dto);

            var dataToCheck = await GetByIdAsync(dto.Id);

            if (dataToCheck == null)
            {
                dto.PackageId = dto.PackageId;
                dto.InvoiceId = dto.InvoiceId;
                dto.IncentiveSA = dto.IncentiveSA;
                dto.IncentiveTech = dto.IncentiveTech;
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