using Microsoft.AspNetCore.Components;
using Rapide.Common.Helpers;
using Rapide.Contracts.Services;
using Rapide.Web.Components.Utilities;
using Rapide.Web.Models;

namespace Rapide.Web.Components.Pages.Operations
{
    public partial class JobsBoardDetails
    {
        #region Parameters
        #endregion

        #region Dependency Injection
        [Inject]
        private IEstimateService EstimateService { get; set; }
        [Inject]
        private IJobOrderService JobOrderService { get; set; }
        #endregion

        #region Private Properties
        private List<EstimateModel> EstimateRequestModel = new List<EstimateModel>();
        private List<JobOrderModel> JobOrderRequestModel = new List<JobOrderModel>();
        private List<JobOrderModel> JobOrderCompletedRequestModel = new List<JobOrderModel>();

        private bool IsLoading { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            #region Estimate List
            var estimateList = await EstimateService.GetAllEstimateAsync();

            if (estimateList == null)
            {
                IsLoading = false;
                return;
            }

            var openEstimates = estimateList.Where(x => x.JobStatus.Name.Equals(Constants.JobStatus.Open));

            foreach (var ul in openEstimates)
            {
                EstimateRequestModel.Add(new EstimateModel()
                {
                    Id = ul.Id,
                    ReferenceNo = ul.ReferenceNo,
                    Summary = ul.Summary,
                    Customer = ul.Customer.Map<CustomerModel>(),
                    Vehicle = new VehiclesModel()
                    {
                        Id = ul.Vehicle.Id,
                        VehicleModel = new VehicleModelModel()
                        {
                            Id = ul.Vehicle.VehicleModel.Id,
                            Name = ul.Vehicle.VehicleModel.Name,
                            VehicleMake = new VehicleMakeModel()
                            {
                                Id = ul.Vehicle.VehicleModel.VehicleMake.Id,
                                Name = ul.Vehicle.VehicleModel.VehicleMake.Name,
                                Description = ul.Vehicle.VehicleModel.VehicleMake.Description
                            }
                        },
                        PlateNo = ul.Vehicle.PlateNo
                    },
                    TotalAmount = ul.TotalAmount,
                    TransactionDate = ul.TransactionDate,
                    JobStatus = ul.JobStatus.Map<JobStatusModel>()
                });
            }
            #endregion

            #region In Progress Job Order List
            var jobOrderList = await JobOrderService.GetAllJobOrderAsync();

            if (jobOrderList == null)
            {
                IsLoading = false;
                return;
            }

            var openJobOrder = jobOrderList.Where(x => x.JobStatus.Name.Equals(Constants.JobStatus.Open));

            foreach (var ul in openJobOrder)
            {
                JobOrderRequestModel.Add(new JobOrderModel()
                {
                    Id = ul.Id,
                    ReferenceNo = ul.ReferenceNo,
                    Summary = ul.Summary,
                    Customer = ul.Customer.Map<CustomerModel>(),
                    Vehicle = new VehiclesModel()
                    {
                        Id = ul.Vehicle.Id,
                        VehicleModel = new VehicleModelModel()
                        {
                            Id = ul.Vehicle.VehicleModel.Id,
                            Name = ul.Vehicle.VehicleModel.Name,
                            VehicleMake = new VehicleMakeModel()
                            {
                                Id = ul.Vehicle.VehicleModel.VehicleMake.Id,
                                Name = ul.Vehicle.VehicleModel.VehicleMake.Name,
                                Description = ul.Vehicle.VehicleModel.VehicleMake.Description
                            }
                        },
                        PlateNo = ul.Vehicle.PlateNo
                    },
                    TotalAmount = ul.TotalAmount,
                    TransactionDate = ul.TransactionDate,
                    JobStatus = ul.JobStatus.Map<JobStatusModel>()
                });
            }
            #endregion

            #region Completed Job Order List
            var jobOrderCompletedList = await JobOrderService.GetAllJobOrderAsync();

            if (jobOrderCompletedList == null)
            {
                IsLoading = false;
                return;
            }

            var completeJobOrder = jobOrderList.Where(x => x.JobStatus.Name.Equals(Constants.JobStatus.Completed));

            foreach (var ul in completeJobOrder)
            {
                JobOrderCompletedRequestModel.Add(new JobOrderModel()
                {
                    Id = ul.Id,
                    ReferenceNo = ul.ReferenceNo,
                    Summary = ul.Summary,
                    Customer = ul.Customer.Map<CustomerModel>(),
                    Vehicle = new VehiclesModel()
                    {
                        Id = ul.Vehicle.Id,
                        VehicleModel = new VehicleModelModel()
                        {
                            Id = ul.Vehicle.VehicleModel.Id,
                            Name = ul.Vehicle.VehicleModel.Name,
                            VehicleMake = new VehicleMakeModel()
                            {
                                Id = ul.Vehicle.VehicleModel.VehicleMake.Id,
                                Name = ul.Vehicle.VehicleModel.VehicleMake.Name,
                                Description = ul.Vehicle.VehicleModel.VehicleMake.Description
                            }
                        },
                        PlateNo = ul.Vehicle.PlateNo
                    },
                    TotalAmount = ul.TotalAmount,
                    TransactionDate = ul.TransactionDate,
                    JobStatus = ul.JobStatus.Map<JobStatusModel>()
                });
            }
            #endregion

            await base.OnInitializedAsync();
        }
    }
}