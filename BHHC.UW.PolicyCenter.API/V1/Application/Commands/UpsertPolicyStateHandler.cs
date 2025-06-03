using AutoMapper;
using BHHC.UW.PolicyCenter.Domain.V1.EntityModels;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using DigitalPlatform.Errors.Models.Extension;
using DigitalPlatform.Errors.Models.Models;
using MediatR;

namespace BHHC.UW.PolicyCenter.API.V1.Application.Commands
{
    public class UpsertPolicyStateHandler : IRequestHandler<UpsertPolicyStateCommand, string>
    {
        private readonly IStateRepository _stateRepository;
        private readonly ILogger<UpsertPolicyStateHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UpsertPolicyStateHandler(
            IStateRepository stateRepository,
            ILogger<UpsertPolicyStateHandler> logger,
            IMapper mapper,
            IConfiguration configuration)
        {
            _stateRepository = stateRepository;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<string> Handle(UpsertPolicyStateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to upsert policy state for MgaCode: {MgaCode}, State: {State}", request.MgaCode, request.State);
                var upsertPolicyState = _mapper.Map<UpsertPolicyState>(request);
                var rMessage = await _stateRepository.UpsertPolicyStateAsync(upsertPolicyState);
                //please create new object UpsertPolicyState same like
                _logger.LogInformation("Upsert policy state operation completed for MgaCode: {MgaCode}, State: {State} with message: {RMessage}", request.MgaCode, request.State, rMessage);
                return rMessage;
            }
            catch (Exception ex)
            {
                var errorMessage = _logger.LogCustomError(
                   new CustomError(_configuration, GlobalErrorCategory.Unhandled, "Error upserting policy state for MgaCode: {request.MgaCode}, State: {request.State}", false),
                   LogLevel.Error,
                   ex,
                   nameof(UpsertPolicyStateCommand),
                   null
               );
                throw;
            }
        }
    }
}
