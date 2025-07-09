using MudBlazor;
using Rapide.DTO;

namespace Rapide.Web.Models
{
    public class PettyCashDetailsModel
    {
        public int Id { get; set; }

        public PettyCashModel PettyCash { get; set; }
        public int PettyCashId { get; set; }

        public string Particulars { get; set; }

        public decimal Amount { get; set; }
    }
}