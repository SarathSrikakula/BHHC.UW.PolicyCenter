using app_uwpolicicenter.Models.PolicyStateModels;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.API.V1.Application.Models;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using Microsoft.Extensions.Options;

namespace app_uwpolicicenter.ServiceClients
{
    //please add these 3 methods to IPolicyStatesServiceClient
    public class PolicyCenterServiceClient : AzureServiceClient, IPolicyStatesServiceClient
    {
        private readonly ILogger<PolicyCenterServiceClient> _logger;
        private readonly string _policyDetailURL;
        private readonly IConfiguration _configuration;
        private readonly PolicyStatesApiPaths _apiPaths;

        public PolicyCenterServiceClient(
            ILogger<PolicyCenterServiceClient> logger,
            IOptions<MicroServiceBaseURLCollection> microServiceBaseURLCollection,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IOptions<PolicyStatesApiPaths> apiPaths,
            string microServiceName = "UW.PolicyCenter.Api"
        ) : base(microServiceBaseURLCollection, httpContextAccessor, configuration, microServiceName)
        {
            _logger = logger;
            _configuration = configuration;
            _apiPaths = apiPaths.Value;
        }

        // Example service call method for GetPolicyStates (GET)
        public async Task<ApiResponse<IEnumerable<UWStateDTO>>> GetPolicyStatesAsync(string policyId)
        {
            try
            {
                var response = await GetAsync<ApiResponse<IEnumerable<UWStateDTO>>>(_apiPaths.PolicyStatesGetURL + policyId, null);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetPolicyStatesAsync)} failed.");
                throw;
            }
        }

        // Example service call method for UpsertPolicyState (POST)
        public async Task<ApiResponse<string>> UpsertPolicyStateAsync(UpsertPolicyStateCommandRequest request)
        {
            try
            {
                var response = await PostAsync<ApiResponse<string>>(_apiPaths.PolicyStateUpsertURL, request,null);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UpsertPolicyStateAsync)} failed.");
                throw;
            }
        }

      

        // Example service call method for GetAvailableStates (GET)
        public async Task<IEnumerable<ReferenceStateDTO>> GetAvailableStatesAsync(string policyId)
        {
            try
            {
                var response = await GetAsync<IEnumerable<ReferenceStateDTO>>(_apiPaths.AvailableStatesGetURL + policyId, null);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetAvailableStatesAsync)} failed.");
                throw;
            }
        }

        //add unit test cases for above method consider mostly mockinng as there are no solid logic, but make sure the request and response objects are created with all data so i will get the coverage also this code should go into their test class
        //only write 
        
    }

    public class MicroServiceBaseURLCollection
    {
    }

    public class AzureServiceClient
    {
        private IOptions<MicroServiceBaseURLCollection> microServiceBaseURLCollection;
        private IHttpContextAccessor httpContextAccessor;
        private IConfiguration configuration;
        private string microServiceName;

        public AzureServiceClient(IOptions<MicroServiceBaseURLCollection> microServiceBaseURLCollection, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, string microServiceName)
        {
            this.microServiceBaseURLCollection = microServiceBaseURLCollection;
            this.httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;
            this.microServiceName = microServiceName;
        }
        protected Task<T> GetAsync<T>(string v, object value)
        {
            throw new NotImplementedException();
        }
        protected Task<T> PostAsync<T>(string v, UpsertPolicyStateCommandRequest request, object value)
        {
            throw new NotImplementedException();
        }
    }
    public class  DigitalPlatform
    {
      

    }
}


