using Rapide.Common.Helpers;
using Rapide.Contracts.Repositories;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Entities;
using System.Linq.Expressions;

namespace Rapide.Services
{
    public class JobStatusService(IJobStatusRepo repo) : BaseService<JobStatus, JobStatusDTO>(repo), IJobStatusService
    {
        public async Task<List<JobStatusDTO>> GetAllAsync()
        {
            try
            {
                List<JobStatusDTO> dtoList = new List<JobStatusDTO>();

                var entity = await repo.GetAllAsync();

                if (entity == null)
                    return null;

                foreach (var e in entity)
                    dtoList.Add(e.Map<JobStatusDTO>());
                
                return dtoList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override async Task<JobStatusDTO?> GetAsync(Expression<Func<JobStatus, bool>> predicate)
        {
            try
            {
                var entity = await base.GetAsync(predicate);

                if (entity == null)
                    return null;

                return entity.Map<JobStatusDTO>();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}