using AutoMapper;
using BHHC.UW.PolicyCenter.Domain.V1.EntityModels;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using MediatR;

namespace BHHC.UW.PolicyCenter.API.V1.Application.Commands
{
    public class UpsertPolicyStateHandler : IRequestHandler<UpsertPolicyStateCommand, string>
    {
        private readonly IStateRepository _stateRepository;
        private readonly ILogger<UpsertPolicyStateHandler> _logger;
        private readonly IMapper _mapper;

        public UpsertPolicyStateHandler(IStateRepository stateRepository, ILogger<UpsertPolicyStateHandler> logger, IMapper mapper)
        {
            _stateRepository = stateRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<string> Handle(UpsertPolicyStateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to upsert policy state for MgaCode: {MgaCode}, State: {State}", request.MgaCode, request.State);
                var uwStateEntity = _mapper.Map<UWStateEntity>(request);
                var rMessage = await _stateRepository.UpsertPolicyStateAsync(uwStateEntity);
                _logger.LogInformation("Upsert policy state operation completed for MgaCode: {MgaCode}, State: {State} with message: {RMessage}", request.MgaCode, request.State, rMessage);
                return rMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting policy state for MgaCode: {MgaCode}, State: {State}", request.MgaCode, request.State);
                throw;
            }
        }
    }
}
