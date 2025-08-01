using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.Web.Helpers;
using Rapide.Web.Models;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class CustomerSalesHistoryComponent
    {
        #region Parameters
        [Parameter]
        public string? CustomerIdParam { get; set; }
        #endregion

        #region Dependency Injection
        [Inject]
        private IInvoiceService? InvoiceService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        #endregion

        #region Private Properties
        private List<InvoiceModel> InvoiceRequestModel = new List<InvoiceModel>();
        private MudDataGrid<InvoiceModel> dataGrid;
        private string searchString;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            if (string.IsNullOrEmpty(CustomerIdParam))
                return;

            var customerId =  int.Parse(CustomerIdParam);

            var dataList = await InvoiceService.GetAllInvoiceByCustomerIdAsync(customerId);

            if (dataList == null)
            {
                return;
            }

            foreach (var ul in dataList)
            {
                InvoiceRequestModel.Add(new InvoiceModel()
                {
                    Id = ul.Id,
                    Customer = new CustomerModel()
                    {
                        Id = ul.Customer.Id,
                        FirstName = ul.Customer.FirstName,
                        LastName = ul.Customer.LastName
                    },
                    InvoiceNo = ul.InvoiceNo,
                    InvoiceDate = ul.InvoiceDate,
                    AdvisorUser = new UserModel()
                    { 
                        Id = ul.AdvisorUser.Id,
                        FirstName = ul.AdvisorUser.FirstName,
                        LastName = ul.AdvisorUser.LastName
                    },
                    DueDate = ul.DueDate,
                    JobOrder = new JobOrderModel()
                    {
                        Id = ul.JobOrder.Id,
                        ReferenceNo = ul.JobOrder.ReferenceNo
                    },
                    TotalAmount = ul.TotalAmount,
                });
            }
        }

        private async Task<GridData<InvoiceModel>> ServerReload(GridState<InvoiceModel> state)
        {
            IEnumerable<InvoiceModel> data = new List<InvoiceModel>();
            data = InvoiceRequestModel.OrderByDescending(x => x.Id);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.InvoiceNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if ($"{element.Customer.FirstName} {element.Customer.MiddleName} {element.Customer.LastName}".Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.JobOrder.ReferenceNo.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.InvoiceDate.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.TotalAmount.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.JobStatus.Name.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }).ToArray();

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(InvoiceModel.InvoiceNo):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.InvoiceNo
                        );
                        break;
                    case nameof(InvoiceModel.Customer.FirstName):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Customer.FirstName
                        );
                        break;
                    case nameof(InvoiceModel.JobOrder.ReferenceNo):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.JobOrder.ReferenceNo
                        );
                        break;
                    case nameof(InvoiceModel.DueDate):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.DueDate
                        );
                        break;
                    case nameof(InvoiceModel.TotalAmount):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.TotalAmount
                        );
                        break;
                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<InvoiceModel>
            {
                TotalItems = totalItems,
                Items = pagedData
            };
        }

        private void OnAddNewClick()
        {
            NavigationManager.NavigateToCustom("/operations/JobOrders/add", true);
        }

        private Task OnSearch(string text)
        {
            searchString = text;
            return dataGrid.ReloadServerData();
        }
    }
}