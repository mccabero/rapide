using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Contracts.Services
{
    public interface IBaseService<TEntity, TDto>
            where TEntity : BaseEntity
            where TDto : BaseDTO
    {
        Task<TDto> CreateAsync(TDto dto);

        Task DeleteAsync(long id);
        
        Task<TDto?> GetAsync(Expression<Func<TEntity, bool>> predicate);
        
        Task<TDto?> GetByIdAsync(long id);
        
        Task<TDto> UpdateAsync(TDto dto);
    }
}