using AutoMapper;
using Rapide.DTO;
using Rapide.Entities;
using Rapide.Web.Models;

namespace Rapide.Web.Helpers
{
    public static class MappingWebHelper
    {
        public static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CustomerModel, CustomerDTO>();
                cfg.CreateMap<CustomerDTO, CustomerModel>();

                cfg.CreateMap<VehicleModel, VehicleDTO>();
                cfg.CreateMap<VehicleDTO, VehicleModel>();
                cfg.CreateMap<VehicleModelModel, VehicleModelDTO>();
                cfg.CreateMap<VehicleModelDTO, VehicleModelModel>();
                cfg.CreateMap<VehicleMakeModel, VehicleMakeDTO>();
                cfg.CreateMap<VehicleMakeDTO, VehicleMakeModel>();

                cfg.CreateMap<ParameterModel, ParameterDTO>();
                cfg.CreateMap<ParameterDTO, ParameterModel>();
                cfg.CreateMap<ParameterGroupModel, ParameterGroupDTO>();
                cfg.CreateMap<ParameterGroupDTO, ParameterGroupModel>();
            });
            var mapper = map.CreateMapper();
            return mapper;
        }
    }
}
