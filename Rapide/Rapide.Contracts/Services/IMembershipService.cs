using Rapide.DTO;
using Rapide.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapide.Contracts.Services
{
    public interface IMembershipService : IBaseService<Membership, MembershipDTO>
    {

        Task<List<MembershipDTO>> GetAllMembershipAsync();
    }
}
