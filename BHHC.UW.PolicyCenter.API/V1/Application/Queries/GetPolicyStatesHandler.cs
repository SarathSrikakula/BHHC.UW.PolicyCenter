using AutoMapper;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.Domain.V1.EntityModels;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using MediatR;
using static BHHC.UW.PolicyCenter.API.PolicyCenterException.HandledException;

namespace BHHC.UW.PolicyCenter.API.V1.Application.Queries
{
    public class GetPolicyStatesHandler : IRequestHandler<GetPolicyStatesQuery, IEnumerable<UWStateDTO>>
    {
        private readonly IStateRepository _stateRepository;
        private readonly ILogger<GetPolicyStatesHandler> _logger;
        private readonly IMapper _mapper;

        public GetPolicyStatesHandler(IStateRepository stateRepository, ILogger<GetPolicyStatesHandler> logger, IMapper mapper)
        {
            _stateRepository = stateRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UWStateDTO>> Handle(GetPolicyStatesQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.MgaCode))
            {
                throw new BusinessLogicException("Policy ID (MgaCode) is required to get policy states.");
            }

            try
            {
                _logger.LogInformation("Retrieving policy states for MgaCode: {MgaCode}", request.MgaCode);
                var entityStates = await _stateRepository.GetPolicyStatesAsync(request.MgaCode);
                var states = _mapper.Map<IEnumerable<UWStateDTO>>(entityStates);
                _logger.LogInformation("Successfully retrieved {Count} policy states for MgaCode: {MgaCode}", states?.Count() ?? 0, request.MgaCode);
                return states;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving policy states for MgaCode: {MgaCode}", request.MgaCode);
                throw;
            }
        }
    }
}
