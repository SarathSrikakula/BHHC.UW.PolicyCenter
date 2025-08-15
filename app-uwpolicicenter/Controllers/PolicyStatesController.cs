using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Antiforgery;
using app_uwpolicicenter.ServiceClients;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.API.V1.Application.Queries;
using FluentValidation;

namespace app_uwpolicicenter.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PolicyStatesController : ControllerBase
    {
        private readonly ILogger<PolicyStatesController> _logger;
        private readonly IAntiforgery _antiforgery;
        private readonly IConfiguration _configuration;
        private readonly IPolicyStatesServiceClient _policyCenterServiceClient;

        public PolicyStatesController(
            IAntiforgery antiforgery,
            IConfiguration configuration,
        ILogger<PolicyStatesController> logger,
            IPolicyStatesServiceClient policyCenterServiceClient)
        {
            _logger = logger;
            _antiforgery = antiforgery;
            _configuration = configuration;
            _policyCenterServiceClient = policyCenterServiceClient;
        }

        [HttpGet]
        [Route("GetPolicyStatesInfo")]
        public async Task<IActionResult> GetPolicyStatesInfo(string policyId, [FromServices] IValidator<GetPolicyStatesQuery> validator)
        {
            if (string.IsNullOrWhiteSpace(policyId))
                return BadRequest("Invalid policy id");

            try
            {
                var response = await _policyCenterServiceClient.GetPolicyStatesAsync(policyId);
                if (!response.Success)
                {
                    var errorMsg = !string.IsNullOrEmpty(response.ValidationMessage) ? response.ValidationMessage : "Failed to fetch policy states.";
                    return BadRequest(errorMsg);
                }
                return Ok(response.Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching policy states.");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/policy/UpsertPolicyStateInfo
        [HttpPost]
        [Route("UpsertPolicyStateInfo")]
        public async Task<IActionResult> UpsertPolicyStateInfo([FromBody] UpsertPolicyStateCommandRequest request)
        {
            if (request == null)
                return BadRequest("Request body is null");

            try
            {
                var response = await _policyCenterServiceClient.UpsertPolicyStateAsync(request);
                if (!response.Success)
                {
                    var errorMsg = !string.IsNullOrEmpty(response.ValidationMessage) ? response.ValidationMessage : "Failed to upsert policy state.";
                    return BadRequest(errorMsg);
                }
                return Ok(response.Result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while upserting policy state.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/policy/GetAvailableStatesInfo
        [HttpGet]
        [Route("GetAvailableStatesInfo")]
        public async Task<IActionResult> GetAvailableStatesInfo(string policyId)
        {
            if (string.IsNullOrWhiteSpace(policyId))
                return BadRequest("Invalid policy id");

            try
            {
                var states = await _policyCenterServiceClient.GetAvailableStatesAsync(policyId);
                return Ok(states);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching available states.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}


