using AutoMapper;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.Domain.V1.EntityModels;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using DigitalPlatform.Errors.Models.Extension;
using DigitalPlatform.Errors.Models.Models;
using MediatR;
using System.Net;
using static BHHC.UW.PolicyCenter.API.PolicyCenterException.HandledException;

namespace BHHC.UW.PolicyCenter.API.V1.Application.Queries
{
    public class GetPolicyStatesHandler : IRequestHandler<GetPolicyStatesQuery, IEnumerable<PolicyAssignedStatesDTO1>>
    {
        private readonly IStateRepository _stateRepository;
        private readonly ILogger<GetPolicyStatesHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public GetPolicyStatesHandler(
           IStateRepository stateRepository,
           ILogger<GetPolicyStatesHandler> logger,
           IMapper mapper,
           IConfiguration configuration)
        {
            _stateRepository = stateRepository;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<IEnumerable<PolicyAssignedStatesDTO1>> Handle(GetPolicyStatesQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.MgaCode))
            {
                throw new BusinessLogicException("Policy Code is required to get policy states.");
            }

            try
            {
                _logger.LogInformation("Retrieving policy states for Policy Code: {MgaCode}", request.MgaCode);
                var entityStates = await _stateRepository.GetPolicyStatesAsync(request.MgaCode);
                var states = _mapper.Map<IEnumerable<PolicyAssignedStatesDTO1>>(entityStates);

                _logger.LogInformation("Successfully retrieved {Count} policy states for MgaCode: {MgaCode}", states?.Count() ?? 0, request.MgaCode);
                return states;
            }
            catch (Exception ex)
            {
                var errorMessage = _logger.LogCustomError(
                   new CustomError(_configuration, GlobalErrorCategory.Unhandled, "Error retrieving policy states for Policy Code: {MgaCode}", false),
                   LogLevel.Error,
                   ex,
                   nameof(GetPolicyStatesQuery),
                   null
               );
                throw;
            }
        }
    }
}
