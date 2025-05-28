using AutoMapper;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.Domain.V1.EntityModels;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;

namespace BHHC.UW.PolicyCenter.API.V1.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Request (DTO) to Command mapping
            CreateMap<UpsertPolicyStateCommandRequest, UpsertPolicyStateCommand>();

            // Command to Entity mapping (for passing to repository)
            CreateMap<UpsertPolicyStateCommand, UWStateEntity>()
                .ForMember(dest => dest.BeginDate, opt => opt.MapFrom(src => src.StateBeginDate)); 

            // Entity to DTO mapping (for returning from repository via handler to controller)
            CreateMap<UWStateEntity, UWStateDTO>();
        }
    }
}
