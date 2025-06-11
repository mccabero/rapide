using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting.Server;
using MudBlazor;
using Rapide.Contracts.Services;
using Rapide.DTO;
using Rapide.Web.Models;

namespace Rapide.Web.Components.Pages.Components
{
    public partial class PackageItemComponent
    {
        [Parameter]
        public PackageDTO SelectedPackage { get; set; } = new PackageDTO();

        [Inject]
        private IPackageService PackageService { get; set; }

        private MudDataGrid<PackageDTO> dataGrid;
        private string searchString;
        private List<PackageDTO> PackageRequestModel = new List<PackageDTO>();

        protected override async Task OnInitializedAsync()
        {
            var dataList = await PackageService.GetAllPackageAsync();

            if (dataList == null)
            {
                return;
            }

            foreach (var ul in dataList)
            {
                PackageRequestModel.Add(new PackageDTO()
                {
                    Id = ul.Id,
                    Name = ul.Name,
                    Code = ul.Code,
                    NextServiceReminderDays = ul.NextServiceReminderDays,
                    IncentiveSA = ul.IncentiveSA,
                    IncentiveTech = ul.IncentiveTech,
                    TotalAmount = ul.TotalAmount,
                });
            }

            StateHasChanged();
            await base.OnInitializedAsync();
        }

        private void OnApplyClick(PackageDTO param)
        {
            MudDialog.Close(DialogResult.Ok(param));
        }

        private async Task<GridData<PackageDTO>> ServerReload(GridState<PackageDTO> state)
        {
            IEnumerable<PackageDTO> data = new List<PackageDTO>();
            data = PackageRequestModel.OrderBy(x => x.Code);

            await Task.Delay(300);
            data = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(searchString))
                    return true;
                if (element.Code.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    return true;

                return false;
            }).ToArray();

            var totalItems = data.Count();

            var sortDefinition = state.SortDefinitions.FirstOrDefault();
            if (sortDefinition != null)
            {
                switch (sortDefinition.SortBy)
                {
                    case nameof(PackageDTO.Code):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Id
                        );
                        break;
                    case nameof(PackageDTO.Name):
                        data = data.OrderByDirection(
                            sortDefinition.Descending ? SortDirection.Descending : SortDirection.Ascending,
                            o => o.Name
                        );
                        break;

                }
            }

            var pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

            return new GridData<PackageDTO>
            {
                TotalItems = totalItems,
                Items = pagedData
            };
        }

        private Task OnSearch(string text)
        {
            searchString = text;
            return dataGrid.ReloadServerData();
        }
    }
}
