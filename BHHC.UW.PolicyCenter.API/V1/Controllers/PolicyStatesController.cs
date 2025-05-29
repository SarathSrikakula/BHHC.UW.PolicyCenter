using AutoMapper;
using BHHC.UW.PolicyCenter.API.PolicyCenterException;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.API.V1.Application.Models;
using BHHC.UW.PolicyCenter.API.V1.Application.Queries;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Net;
using static BHHC.UW.PolicyCenter.API.PolicyCenterException.HandledException;
using Microsoft.Extensions.Logging;
using DigitalPlatform.Errors.Models.Models;
using DigitalPlatform.Errors.Models.Extension;

namespace BHHC.UW.PolicyCenter.API.V1.Controllers
{
    [ApiController]
    [Route("api/policies/{policyId}/states")]
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


        // Updated GetPolicyStates method to match the latest API response format
        [HttpGet]
        public async Task<IActionResult> GetPolicyStates(string policyId)
        {
            try
            {
                var query = new GetPolicyStatesQuery { MgaCode = policyId };
                var states = await _mediator.Send(query);

                if (states == null)
                {
       
                    var errorMessage = _logger.LogCustomError(
                        new CustomError(_configuration, GlobalErrorCategory.Database, "4010", true),
                        LogLevel.Warning,
                        new Exception("Policy states not found."),
                        nameof(GetPolicyStates)
                    );

                    return Ok(new ApiResponse<UWStateDTO>
                    {
                        Success = false,
                        Exception = new ApiException(exception: new Exception(errorMessage))
                    });
                }

                var response = new ApiResponse<IEnumerable<UWStateDTO>>
                {
                    Success = true,
                    Result = states
                };

                return Ok(response);
            }
            catch (SqlException ex)
            {
                _logger.LogCustomError(
                    new CustomError(_configuration, GlobalErrorCategory.Database, "1005", false),
                    LogLevel.Error,
                    ex,
                    nameof(GetPolicyStates)
                );

                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse<bool>
                {
                    Success = false,
                    Exception = new ApiException(exception: ex)
                });
            }
            catch (Exception ex)
            {
                _logger.LogCustomError(
                    new CustomError(_configuration, GlobalErrorCategory.Unhandled, "1006", false),
                    LogLevel.Error,
                    ex,
                    nameof(GetPolicyStates)
                );

                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse<bool>
                {
                    Success = false,
                    Exception = new ApiException(exception: ex)
                });
            }
        }

        // Updated UpsertPolicyState method to match the latest API response format
        [HttpPost]
        public async Task<IActionResult> UpsertPolicyState([FromBody] UpsertPolicyStateCommandRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Exception = new ApiException(exception: new Exception("Invalid model state."))
                });
            }

            var command = _mapper.Map<UpsertPolicyStateCommand>(request);

            try
            {
                var rMessage = await _mediator.Send(command);
                var response = new ApiResponse<string>
                {
                    Success = true,
                    Result = rMessage
                };
                return Ok(response);
            }
            catch (BusinessLogicException ex)
            {
                //please add LogCustomError
                _logger.LogCustomError(
                    new CustomError(_configuration, GlobalErrorCategory.Business, "4001", false),
                    LogLevel.Warning,
                    ex,
                    nameof(UpsertPolicyState)
                );

                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Exception = new ApiException(exception: ex)
                });
            }
            catch (SqlException ex)
            {
                _logger.LogCustomError(
                    new CustomError(_configuration, GlobalErrorCategory.Database, "1005", false),
                    LogLevel.Error,
                    ex,
                    nameof(UpsertPolicyState)
                );

                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse<bool>
                {
                    Success = false,
                    Exception = new ApiException(exception: ex)
                });
            }
            catch (Exception ex)
            {
                _logger.LogCustomError(
                    new CustomError(_configuration, GlobalErrorCategory.Unhandled, "1006", false),
                    LogLevel.Error,
                    ex,
                    nameof(UpsertPolicyState)
                );

                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse<bool>
                {
                    Success = false,
                    Exception = new ApiException(exception: ex)
                });
            }
        }

        // Updated GetAvailableStates method to match the latest API response format
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableStates([FromQuery] string mgacode)
        {
            if (string.IsNullOrWhiteSpace(mgacode))
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Exception = new ApiException(exception: new Exception("MgaCode is required to retrieve available states."))
                });
            }

            try
            {
                var query = new GetAvailableStatesQuery { MgaCode = mgacode };
                var states = await _mediator.Send(query);

                if (states == null)
                {
                    var errorMessage = _logger.LogCustomError(
                        new CustomError(_configuration, GlobalErrorCategory.Database, "4011", true),
                        LogLevel.Warning,
                        new Exception("Available states not found."),
                        nameof(GetAvailableStates)
                    );

                    return Ok(new ApiResponse<ReferenceStateDTO>
                    {
                        Success = false,
                        Exception = new ApiException(exception: new Exception(errorMessage))
                    });
                }

                var response = new ApiResponse<IEnumerable<ReferenceStateDTO>>
                {
                    Success = true,
                    Result = states
                };

                return Ok(response);
            }
            catch (SqlException ex)
            {
                _logger.LogCustomError(
                    new CustomError(_configuration, GlobalErrorCategory.Database, "1005", false),
                    LogLevel.Error,
                    ex,
                    nameof(GetAvailableStates)
                );

                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse<bool>
                {
                    Success = false,
                    Exception = new ApiException(exception: ex)
                });
            }
            catch (Exception ex)
            {
                _logger.LogCustomError(
                    new CustomError(_configuration, GlobalErrorCategory.Unhandled, "1006", false),
                    LogLevel.Error,
                    ex,
                    nameof(GetAvailableStates)
                );

                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse<bool>
                {
                    Success = false,
                    Exception = new ApiException(exception: ex)
                });
            }
        }
        //As we have totally changed the PolicyStatesController, we need to update the unit tests totally 
    }
}
