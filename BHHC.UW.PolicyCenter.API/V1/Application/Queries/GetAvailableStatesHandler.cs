using AutoMapper;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using DigitalPlatform.Errors.Models.Extension;
using DigitalPlatform.Errors.Models.Models;
using MediatR;
using static BHHC.UW.PolicyCenter.API.PolicyCenterException.HandledException;

namespace BHHC.UW.PolicyCenter.API.V1.Application.Queries
{
    public class GetAvailableStatesHandler : IRequestHandler<GetAvailableStatesQuery, IEnumerable<PolicyAvailableStatesDTO1>>
    {
        private readonly IStateRepository _stateRepository;
        private readonly ILogger<GetAvailableStatesHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GetAvailableStatesHandler(
            IStateRepository stateRepository,
            ILogger<GetAvailableStatesHandler> logger,
            IMapper mapper,
            IConfiguration configuration)
        {
            _stateRepository = stateRepository;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<IEnumerable<PolicyAvailableStatesDTO1>> Handle(GetAvailableStatesQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.MgaCode))
            {
                throw new BusinessLogicException("MGA Code is required to get available states.");
            }

            try
            {
                _logger.LogInformation("Retrieving available states for Policy Code: {MgaCode}", request.MgaCode);
                var entityStates = await _stateRepository.GetAllAvailableStatesAsync(request.MgaCode);
                var states = _mapper.Map<IEnumerable<PolicyAvailableStatesDTO1>>(entityStates);

                _logger.LogInformation("Successfully retrieved {Count} available states for Policy Code: {MgaCode}", states?.Count() ?? 0, request.MgaCode);
                return states;
            }
            catch (Exception ex)
            {
                var errorMessage = _logger.LogCustomError(
                   new CustomError(_configuration, GlobalErrorCategory.Unhandled, "Error retrieving policy states for Policy Code: {MgaCode}", false),
                   LogLevel.Error,
                   ex,
                   nameof(GetAvailableStatesQuery),
                   null
               );
                throw;
            }
        }
    }
}
