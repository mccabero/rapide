using AutoMapper;
using Rapide.DTO;
using Rapide.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rapide.Common.Helpers
{
    public static class MappingHelper
    {
        public static IMapper InitializeMapper()
        {
            var map = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Customer, CustomerDTO>();
                cfg.CreateMap<CustomerDTO, Customer>();

                cfg.CreateMap<Vehicle, VehicleDTO>();
                cfg.CreateMap<VehicleDTO, Vehicle>();
                cfg.CreateMap<VehicleMakeDTO, VehicleMake>();
                cfg.CreateMap<VehicleMake, VehicleMakeDTO>();
                cfg.CreateMap<VehicleModelDTO, VehicleModel>();
                cfg.CreateMap<VehicleModel, VehicleModelDTO>();

                cfg.CreateMap<ParameterDTO, Parameter>();
                cfg.CreateMap<Parameter, ParameterDTO>();
                cfg.CreateMap<ParameterGroupDTO, ParameterGroup>();
                cfg.CreateMap<ParameterGroup, ParameterGroupDTO>();

                cfg.CreateMap<InspectionDTO, Inspection>();
                cfg.CreateMap<Inspection, InspectionDTO>();

                cfg.CreateMap<JobStatusDTO, JobStatus>();
                cfg.CreateMap<JobStatus, JobStatusDTO>();

                cfg.CreateMap<UserDTO, User>();
                cfg.CreateMap<User, UserDTO>();

                cfg.CreateMap<RoleDTO, Role>();
                cfg.CreateMap<Role, RoleDTO>();

                cfg.CreateMap<ServiceGroupDTO, ServiceGroup>();
                cfg.CreateMap<ServiceGroup, ServiceGroupDTO>();

                cfg.CreateMap<ParameterDTO, Parameter>();
                cfg.CreateMap<Parameter, ParameterDTO>();
                
                
            });
            var mapper = map.CreateMapper();
            return mapper;
        }
    }
}
