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
    public class ExpensesService(IExpensesRepo repo) : BaseService<Expenses, ExpensesDTO>(repo), IExpensesService
    {
        private static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Expenses, ExpensesDTO>();
                cfg.CreateMap<JobStatus, JobStatusDTO>();
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<Parameter, ParameterDTO>();
                cfg.CreateMap<ParameterGroup, ParameterGroupDTO>();
                cfg.CreateMap<Role, RoleDTO>();

                cfg.CreateMap<RoleDTO, Role>();
                cfg.CreateMap<ParameterGroupDTO, ParameterGroup>();
                cfg.CreateMap<ParameterDTO, Parameter>();
                cfg.CreateMap<ExpensesDTO, Expenses>();
                cfg.CreateMap<JobStatusDTO, JobStatus>();
                cfg.CreateMap<UserDTO, User>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }

        public async Task<List<ExpensesDTO>> GetAllExpensesAsync()
        {
            try
            {
                List<ExpensesDTO> dtoList = new List<ExpensesDTO>();
                var entityList = await repo.GetAllExpensesAsync();

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<ExpensesDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<ExpensesDTO?> GetAsync(Expression<Func<Expenses, bool>> predicate)
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

        public async Task<ExpensesDTO?> GetExpensesByIdAsync(int id)
        {
            try
            {
                var entity = await repo.GetExpensesByIdAsync(id);

                if (entity == null)
                    return null;

                IMapper mapper = InitializeMapper();

                return mapper.Map<ExpensesDTO>(entity);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<ExpensesDTO> CreateAsync(ExpensesDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.PaymentTypeParameter = null;
            dto.ExpenseByUser = null;
            dto.JobStatus = null;

            var createdDto = await base.CreateAsync(dto);

            return createdDto;
        }

        public override async Task<ExpensesDTO> UpdateAsync(ExpensesDTO dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            IMapper mapper = InitializeMapper();

            // Remove FKs. only parent table is tobe inserted
            dto.PaymentTypeParameter = null;
            dto.ExpenseByUser = null;
            dto.JobStatus = null;

            var dtoMap = mapper.Map<Expenses>(dto);

            await base.UpdateByEntityAsync(dtoMap);

            return dto;
        }

        public async Task<List<ExpensesDTO>> GetAllExpensesByExpenseByUserIdAsync(int expenseByUserId)
        {
            try
            {
                List<ExpensesDTO> dtoList = new List<ExpensesDTO>();
                var entityList = await repo.GetAllExpensesByExpenseByUserIdAsync(expenseByUserId);

                if (entityList.IsNullOrEmpty())
                    return null;

                IMapper mapper = InitializeMapper();

                foreach (var e in entityList)
                    dtoList.Add(mapper.Map<ExpensesDTO>(e));

                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}