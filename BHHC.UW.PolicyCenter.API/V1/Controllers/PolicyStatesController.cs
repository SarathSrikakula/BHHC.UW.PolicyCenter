using AutoMapper;
using BHHC.UW.PolicyCenter.API.PolicyCenterException;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.API.V1.Application.Models;
using BHHC.UW.PolicyCenter.API.V1.Application.Queries;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using DigitalPlatform.Errors.Models.Extension;
using DigitalPlatform.Errors.Models.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Net;
using static BHHC.UW.PolicyCenter.API.PolicyCenterException.HandledException;

namespace BHHC.UW.PolicyCenter.API.V1.Controllers
{
    [ApiController]
    [Route("api/policy")]
    public class PolicyStatesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<PolicyStatesController> _logger;
        private readonly IConfiguration _configuration;

        public PolicyStatesController(IMediator mediator, IMapper mapper, ILogger<PolicyStatesController> logger, IConfiguration configuration)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }
        //please add logger and _configuration to the unit test cases
        /// <summary>
        /// Retrieves all states associated with a specific policy.
        /// </summary>
        /// <param name="policyId">The ID of the policy.</param>
        /// <returns>A list of policy-linked states.</returns>
        [HttpGet("policystates")]
        [ProducesResponseType(typeof(IEnumerable<PolicyAssignedStatesDTO1>), 200)]
        [ProducesResponseType(typeof(WebApiExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(WebApiExceptionResponseModel), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetPolicyStates(
            [FromQuery] string policyId,
            [FromServices] IValidator<GetPolicyStatesQuery> validator)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(policyId))
                {
                    return Ok(new ApiResponse<PolicyAssignedStatesDTO1>
                    {
                        Success = false,
                        ValidationMessage = "Invalid PolicyId"
                    });
                }

                var query = new GetPolicyStatesQuery { MgaCode = policyId };
                var validationResult = await validator.ValidateAsync(query);
                if (!validationResult.IsValid)
                {
                    string combinedErrors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Ok(new ApiResponse<string>
                    {
                        Success = false,
                        ValidationMessage = combinedErrors
                    });
                }

                var states = await _mediator.Send(query);
                return Ok(new ApiResponse<IEnumerable<PolicyAssignedStatesDTO1>>
                {
                    Success = true,
                    Result = states
                });
            }
            catch (BusinessLogicException ex)
            {
                return BadRequest(new WebApiExceptionResponseModel
                {
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
            catch (Exception ex)
            {
                var errorMessage = _logger.LogCustomError(
                    new CustomError(_configuration, GlobalErrorCategory.Unhandled, "1001", false),
                    LogLevel.Error,
                    ex,
                    nameof(GetPolicyStates),
                    null
                );

                return StatusCode((int)HttpStatusCode.InternalServerError, errorMessage);
            }
        }

        /// <summary>
        /// Adds or updates a state for a specific policy.
        /// </summary>
        /// <param name="request">The state data to add or update.</param>
        /// <returns>A message indicating the result of the operation.</returns>
        [HttpPost("states")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(WebApiExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(WebApiExceptionResponseModel), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpsertPolicyState(
            [FromBody] UpsertPolicyStateCommandRequest request,
            [FromServices] IValidator<UpsertPolicyStateCommandRequest> validator)
        {
            try
            {
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    string combinedErrors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Ok(new ApiResponse<string>
                    {
                        Success = false,
                        ValidationMessage = combinedErrors
                    });
                }

                var command = _mapper.Map<UpsertPolicyStateCommand>(request);
                var rMessage = await _mediator.Send(command);
                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Result = rMessage
                });
            }
            catch (BusinessLogicException ex)
            {
                return BadRequest(new WebApiExceptionResponseModel
                {
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                });
            }
            catch (Exception ex)
            {
                var errorMessage = _logger.LogCustomError(
                    new CustomError(_configuration, GlobalErrorCategory.Unhandled, "1001", false),
                    LogLevel.Error,
                    ex,
                    nameof(UpsertPolicyState),
                    null
                );

                return StatusCode((int)HttpStatusCode.InternalServerError, errorMessage);
            }
        }

        /// <summary>
        /// Retrieves a list of all available states for dropdowns based on policy-related data.
        /// </summary>
        /// <param name="policyId">The MgaCode (Policy ID) to filter available states.</param>
        /// <param name="validator">Validator for UpsertPolicyStateCommandRequest.</param>
        /// <returns>A list of available states.</returns>
        [HttpGet("availablestates")]
        [ProducesResponseType(typeof(IEnumerable<PolicyAvailableStatesDTO1>), 200)]
        [ProducesResponseType(typeof(WebApiExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(WebApiExceptionResponseModel), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAvailableStates(
            [FromQuery] string policyId,
            [FromServices] IValidator<GetAvailableStatesQuery> validator)
        {
            if (string.IsNullOrWhiteSpace(policyId))
            {
                return BadRequest(new WebApiExceptionResponseModel
                {
                    ExceptionType = nameof(BusinessLogicException),
                    ExceptionMessage = "MgaCode is required to retrieve available states.",
                });
            }

            // Use validator for UpsertPolicyStateCommandRequest
            var validationRequest = new GetAvailableStatesQuery { MgaCode = policyId };
            var validationResult = await validator.ValidateAsync(validationRequest);
            if (!validationResult.IsValid)
            {
                string combinedErrors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Ok(new ApiResponse<string>
                {
                    Success = false,
                    ValidationMessage = combinedErrors
                });
            }

            try
            {
                var query = new GetAvailableStatesQuery { MgaCode = policyId };
                var states = await _mediator.Send(query);
                return Ok(new ApiResponse<IEnumerable<PolicyAvailableStatesDTO1>>
                {
                    Success = true,
                    Result = states
                });
            }
            catch (BusinessLogicException ex)
            {
                return BadRequest(new WebApiExceptionResponseModel
                {
                    ExceptionType = nameof(BusinessLogicException),
                    ExceptionMessage = ex.Message,
                });
            }
            catch (Exception ex)
            {
                var errorMessage = _logger.LogCustomError(
                    new CustomError(_configuration, GlobalErrorCategory.Unhandled, "1001", false),
                    LogLevel.Error,
                    ex,
                    nameof(GetAvailableStates),
                    null
                );

                return StatusCode((int)HttpStatusCode.InternalServerError, errorMessage);
            }
        }
    }
}
//PLEASE USE [FromServices] IValidator<UpsertPolicyStateCommandRequest> validator and it's validation logic in GetAvailableStates same like GetPolicyStates method.
//[HttpGet]
//[Route("GetPolicyHeaderInfo")]
//public async Task<IActionResult> GetPolicyHeaderInfo(string policyCode)
//{
//    if (string.IsNullOrWhiteSpace(policyCode))
//        return BadRequest("Invalid policy code");

//    try
//    {
//        var policy = await _policyCenterServiceClient.GetPolicyDetail(policyCode);
//        return Ok(policy); // or Json(policy), depending on your return type
//    }
//    catch (Exception ex)
//    {
//        _logger.LogError(ex, "Error while fetching policy details.");
//        return StatusCode(500, "Internal server error");
//    }
//}
//above is GetPolicyHeaderInfo is the eg for which we need 3 similar controller methods for the following methods of our current class, GetPolicyHeaderInfo generally will use client to call our controller class methods.
//please remeber i want the format as it is of GetPolicyHeaderInfo

































//public async Task<ApiResponse<List<PolicyDTO>>> GetPolicyDetail(string policyCode)
//{
//    try
//    {
//        // Assuming you don’t need scopes for localhost/testing
//        var response = await GetAsync<ApiResponse<List<PolicyDTO>>>(_policyDetailURL + policyCode, null);
//        return response;
//    }
//    catch (Exception ex)
//    {
//        _logger.LogError(ex, $"{nameof(GetPolicyDetail)} failed.");
//        throw;
//    }
//}
//
