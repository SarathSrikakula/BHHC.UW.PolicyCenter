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
                .ForMember(dest => dest.STBEGIN, opt => opt.MapFrom(src => src.StateBeginDate))
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.ST_Tin, opt => opt.MapFrom(src => src.StateTin))
                .ForMember(dest => dest.RiskId, opt => opt.MapFrom(src => src.RiskId))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Entity to DTO mapping (for returning from repository via handler to controller)
            CreateMap<UWStateEntity, PolicyAssignedStatesDTO>();
            //property mapping for the above DTO
            CreateMap<UWStateEntity, PolicyAssignedStatesDTO>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.StateName))
                .ForMember(dest => dest.StateEmployerCode, opt => opt.MapFrom(src => src.ST_Tin))
                .ForMember(dest => dest.StateEffectiveDate, opt => opt.MapFrom(src => src.STBEGIN))
                .ForMember(dest => dest.BureauId, opt => opt.MapFrom(src => src.RiskId))
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<PolicyAssignedStatesDTO, PolicyAssignedStatesDTO1>();
            CreateMap<PolicyAvailableStatesDTO, PolicyAvailableStatesDTO1>();
            //please add mapping for all the StateRepository methods that return DTOs
            CreateMap<UWStateEntity, PolicyAssignedStatesDTO>()
                .ForMember(dest => dest.PolicyCode, opt => opt.MapFrom(src => src.MgaCode))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.StateEffectiveDate, opt => opt.MapFrom(src => src.STBEGIN))
                .ForMember(dest => dest.StateEmployerCode, opt => opt.MapFrom(src => src.ST_Tin))
                .ForMember(dest => dest.BureauId, opt => opt.MapFrom(src => src.RiskId))
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.StateName));

            // ReferenceStateEntity -> PolicyAvailableStatesDTO
            CreateMap<ReferenceStateEntity, PolicyAvailableStatesDTO>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.StateName));

            // UpsertPolicyState -> UWStateEntity
            CreateMap<UpsertPolicyState, UWStateEntity>()
                .ForMember(dest => dest.MgaCode, opt => opt.MapFrom(src => src.MgaCode))
                .ForMember(dest => dest.STBEGIN, opt => opt.MapFrom(src => src.StateBeginDate))
                .ForMember(dest => dest.ST_Tin, opt => opt.MapFrom(src => src.StateTin))
                .ForMember(dest => dest.RiskId, opt => opt.MapFrom(src => src.RiskId))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                //.ForAllOtherMembers(opt => opt.Ignore());
        }
    }
}
