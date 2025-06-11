using Rapide.Contracts.Services;

namespace Rapide.Web.Helpers
{
    public static class ReferenceNumberHelper
    {
        public static async Task<string> GetRNInspection(IInspectionService service)
        {
            var data = await service.GetAllInspectionAsync();
            var lastId = 1;

            if (data != null)
            {
                lastId = data.Count;
                lastId++;

                return $"VI{lastId.ToString("0000000")}";
            }

            return string.Empty;
        }

        public static async Task<string> GetRNEstimate(IEstimateService service)
        {
            var data = await service.GetAllEstimateAsync();
            var lastId = 1;

            if (data != null)
            {
                lastId = data.Count;
                lastId++;

                return $"EST{lastId.ToString("0000000")}";
            }

            return string.Empty;
        }

        public static async Task<string> GetRNJobOrder(IJobOrderService service)
        {
            var data = await service.GetAllJobOrderAsync();
            var lastId = 1;

            if (data != null)
            {
                lastId = data.Count;
                lastId++;

                return $"JO{lastId.ToString("0000000")}";
            }

            return string.Empty;
        }

        public static async Task<string> GetRNInvoice(IInvoiceService service)
        {
            var data = await service.GetAllInvoiceAsync();
            var lastId = 1;

            if (data != null)
            {
                lastId = data.Count;
                lastId++;

                return $"INV{lastId.ToString("0000000")}";
            }

            return string.Empty;
        }

        public static async Task<string> GetRNDeposit(IDepositService service)
        {
            var data = await service.GetAllDepositAsync();
            var lastId = 1;

            if (data != null)
            {
                lastId = data.Count;
                lastId++;

                return $"DP{lastId.ToString("0000000")}";
            }

            return string.Empty;
        }

        public static async Task<string> GetRNPayment(IPaymentService service)
        {
            var data = await service.GetAllPaymentAsync();
            var lastId = 1;

            if (data != null)
            {
                lastId = data.Count;
                lastId++;

                return $"PY{lastId.ToString("0000000")}";
            }

            return string.Empty;
        }

        public static async Task<string> GetRNQuickSales(IQuickSalesService service)
        {
            var data = await service.GetAllQuickSalesAsync();
            var lastId = 1;

            if (data != null)
            {
                lastId = data.Count;
                lastId++;

                return $"QS{lastId.ToString("0000000")}";
            }

            return string.Empty;
        }

        public static async Task<string> GetRNExpenses(IExpensesService service)
        {
            var data = await service.GetAllExpensesAsync();
            var lastId = 1;

            if (data != null)
            {
                lastId = data.Count;
                lastId++;

                return $"EXP{lastId.ToString("0000000")}";
            }

            return string.Empty;
        }
    }
}
