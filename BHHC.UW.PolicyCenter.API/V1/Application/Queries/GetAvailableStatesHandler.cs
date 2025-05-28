using AutoMapper;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using MediatR;
using static BHHC.UW.PolicyCenter.API.PolicyCenterException.HandledException;

namespace BHHC.UW.PolicyCenter.API.V1.Application.Queries
{
    public class GetAvailableStatesHandler : IRequestHandler<GetAvailableStatesQuery, IEnumerable<ReferenceStateDTO>>
    {
        private readonly IStateRepository _stateRepository;
        private readonly ILogger<GetAvailableStatesHandler> _logger;
        private readonly IMapper _mapper;

        public GetAvailableStatesHandler(IStateRepository stateRepository, ILogger<GetAvailableStatesHandler> logger, IMapper mapper)
        {
            _stateRepository = stateRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReferenceStateDTO>> Handle(GetAvailableStatesQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.MgaCode))
            {
                throw new BusinessLogicException("MGA Code is required to get available states.");
            }

            try
            {
                _logger.LogInformation("Retrieving available states for MgaCode: {MgaCode}", request.MgaCode);
                var entityStates = await _stateRepository.GetAllAvailableStatesAsync(request.MgaCode);
                var states = _mapper.Map<IEnumerable<ReferenceStateDTO>>(entityStates);
                _logger.LogInformation("Successfully retrieved {Count} available states for MgaCode: {MgaCode}", states?.Count() ?? 0, request.MgaCode);
                return states;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available states for MgaCode: {MgaCode}", request.MgaCode);
                throw;
            }
        }
    }
}
