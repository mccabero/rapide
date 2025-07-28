using Rapide.Entities;
using Rapide.DTO;

namespace Rapide.Contracts.Repositories
{
    public interface IMembershipRepo : IBaseRepo<Membership>
    {

        Task<List<Membership>> GetAllMembershipAsync();
    }
}
