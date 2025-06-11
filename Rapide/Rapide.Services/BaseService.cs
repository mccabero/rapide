using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Services
{
    public class BaseService<TEntity, TDto>(IBaseRepo<TEntity> repo) : IBaseService<TEntity, TDto> where TDto : BaseDTO
            where TEntity : BaseEntity
    {
        protected IBaseRepo<TEntity> Repo { get; } = repo;

        public virtual async Task<TDto?> GetByIdAsync(long id)
        {
            var entity = await Repo.GetAsync(x => x.Id == id);
            
            if (entity == null)
                return null;

            return entity.Map<TDto>();
        }

        public virtual async Task<TDto?> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                var entity = await Repo.GetAsync(predicate);
                
                if (entity == null)
                    return null;

                return entity.Map<TDto>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public virtual async Task<TDto> CreateAsync(TDto dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            var entity = dto.Map<TEntity>();
            var savedUser = await Repo.CreateAsync(entity);
            return savedUser.Map<TDto>();
        }

        public virtual async Task<TEntity> CreateByEntityAsync(TEntity entity)
        {
            return await Repo.CreateAsync(entity!);
        }

        public virtual async Task<TDto> UpdateAsync(TDto dto)
        {
            // Convert everything to uppercase.
            dto = dto.MemberwiseApply((string s) => string.IsNullOrEmpty(s) ? s : s.ToUpper());

            var entity = dto.Map<TEntity>();
            var data = await Repo.UpdateAsync(entity!);

            return data.Map<TDto>();
        }

        public virtual async Task<TEntity> UpdateByEntityAsync(TEntity entity)
        {
            return await Repo.UpdateAsync(entity!);
        }

        public virtual Task DeleteAsync(long id)
        {
            return Repo.DeleteAsync(id);
        }
    }
}