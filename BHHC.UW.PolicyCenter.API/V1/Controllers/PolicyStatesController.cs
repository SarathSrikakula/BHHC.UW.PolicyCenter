using AutoMapper;
using BHHC.UW.PolicyCenter.API.PolicyCenterException;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.API.V1.Application.Models;
using BHHC.UW.PolicyCenter.API.V1.Application.Queries;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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

        public PolicyStatesController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all states associated with a specific policy.
        /// </summary>
        /// <param name="policyId">The ID of the policy.</param>
        /// <returns>A list of policy-linked states.</returns>
        [HttpGet("{policyid}/states")]
        [ProducesResponseType(typeof(IEnumerable<UWStateDTO>), 200)]
        [ProducesResponseType(typeof(WebApiExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(WebApiExceptionResponseModel), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetPolicyStates(
            string policyId,
            [FromServices] IValidator<GetPolicyStatesQuery> validator)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(policyId))
                {
                    return Ok(new ApiResponse<UWStateDTO>
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
                return Ok(new ApiResponse<IEnumerable<UWStateDTO>>
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
                return StatusCode((int)HttpStatusCode.InternalServerError, new WebApiExceptionResponseModel
                {
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = "An unexpected error occurred while retrieving policy states."
                });
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
                return StatusCode((int)HttpStatusCode.InternalServerError, new WebApiExceptionResponseModel
                {
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = "An unexpected error occurred while saving state data."
                });
            }
        }

        /// <summary>
        /// Retrieves a list of all available states for dropdowns based on policy-related data.
        /// </summary>
        /// <param name="mgacode">The MgaCode (Policy ID) to filter available states.</param>
        /// <returns>A list of available states.</returns>
        [HttpGet("{policyId}/statesavailable")]
        [ProducesResponseType(typeof(IEnumerable<ReferenceStateDTO>), 200)]
        [ProducesResponseType(typeof(WebApiExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(WebApiExceptionResponseModel), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAvailableStates([FromQuery] string policyId)
        {
            if (string.IsNullOrWhiteSpace(policyId))
            {
                return BadRequest(new WebApiExceptionResponseModel
                {
                    ExceptionType = nameof(BusinessLogicException),
                    ExceptionMessage = "MgaCode is required to retrieve available states.",
                });
            }

            try
            {
                var query = new GetAvailableStatesQuery { MgaCode = policyId };
                var states = await _mediator.Send(query);
                return Ok(states);
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
                return StatusCode((int)HttpStatusCode.InternalServerError, new WebApiExceptionResponseModel
                {
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = "An unexpected error occurred while retrieving available states.",
                });
            }
        }
    }
}
