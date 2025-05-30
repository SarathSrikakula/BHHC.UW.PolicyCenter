using app_uwpolicicenter.Controllers;
using app_uwpolicicenter.ServiceClients;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.API.V1.Application.Models;
using BHHC.UW.PolicyCenter.API.V1.Application.Queries;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace app_uwpolicicenter.Tests
{

    public class PolicyStatesControllerTests
    {
        private readonly Mock<ILogger<PolicyStatesController>> _loggerMock = new();
        private readonly Mock<IAntiforgery> _antiforgeryMock = new();
        private readonly Mock<IConfiguration> _configurationMock = new();
        private readonly Mock<IPolicyStatesServiceClient> _serviceClientMock = new();
        private readonly PolicyStatesController _controller;

        public PolicyStatesControllerTests()
        {
            _controller = new PolicyStatesController(
                _antiforgeryMock.Object,
                _configurationMock.Object,
                _loggerMock.Object,
                _serviceClientMock.Object
            );
        }

        private static UpsertPolicyStateCommandRequest GetValidUpsertRequest() => new UpsertPolicyStateCommandRequest
        {
            MgaCode = "MGA123",
            StateBeginDate = DateTime.UtcNow,
            StateTin = "TIN123",
            RiskId = "RISK1",
            State = "TX"
        };

        private static UWStateDTO GetValidUWStateDTO() => new UWStateDTO
        {
            MgaCode = "MGA123",
            State = "TX",
            BeginDate = DateTime.UtcNow,
            StateTin = "TIN123",
            RiskId = "RISK1",
            StateName = "Texas"
        };

        private static ReferenceStateDTO GetValidReferenceStateDTO() => new ReferenceStateDTO
        {
            State = "TX",
            StateName = "Texas"
        };

        [Fact]
        public async Task GetPolicyStatesInfo_ReturnsBadRequest_WhenPolicyIdIsNullOrWhitespace()
        {
            var validatorMock = new Mock<IValidator<GetPolicyStatesQuery>>();
            var result = await _controller.GetPolicyStatesInfo("", validatorMock.Object);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetPolicyStatesInfo_ReturnsBadRequest_WhenServiceReturnsFailure()
        {
            var validatorMock = new Mock<IValidator<GetPolicyStatesQuery>>();
            _serviceClientMock.Setup(s => s.GetPolicyStatesAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApiResponse<IEnumerable<UWStateDTO>>
                {
                    Success = false,
                    ValidationMessage = "Validation failed",
                    Result = null
                });
            var result = await _controller.GetPolicyStatesInfo("POL123", validatorMock.Object);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Validation failed", badRequest.Value);
        }

        [Fact]
        public async Task GetPolicyStatesInfo_ReturnsInternalServerError_OnException()
        {
            var validatorMock = new Mock<IValidator<GetPolicyStatesQuery>>();
            _serviceClientMock.Setup(s => s.GetPolicyStatesAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Test exception"));
            var result = await _controller.GetPolicyStatesInfo("POL123", validatorMock.Object);
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpsertPolicyStateInfo_ReturnsBadRequest_WhenRequestIsNull()
        {
            var result = await _controller.UpsertPolicyStateInfo(null);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpsertPolicyStateInfo_ReturnsBadRequest_WhenServiceReturnsFailure()
        {
            var request = GetValidUpsertRequest();
            _serviceClientMock.Setup(s => s.UpsertPolicyStateAsync(It.IsAny<UpsertPolicyStateCommandRequest>()))
                .ReturnsAsync(new ApiResponse<string>
                {
                    Success = false,
                    ValidationMessage = "Upsert failed",
                    Result = null
                });
            var result = await _controller.UpsertPolicyStateInfo(request);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Upsert failed", badRequest.Value);
        }

        [Fact]
        public async Task UpsertPolicyStateInfo_ReturnsInternalServerError_OnException()
        {
            var request = GetValidUpsertRequest();
            _serviceClientMock.Setup(s => s.UpsertPolicyStateAsync(It.IsAny<UpsertPolicyStateCommandRequest>()))
                .ThrowsAsync(new Exception("Test exception"));
            var result = await _controller.UpsertPolicyStateInfo(request);
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        [Fact]
        public async Task GetAvailableStatesInfo_ReturnsBadRequest_WhenPolicyIdIsNullOrWhitespace()
        {
            var result = await _controller.GetAvailableStatesInfo("");
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAvailableStatesInfo_ReturnsOk_WithStates()
        {
            var states = new List<ReferenceStateDTO> { GetValidReferenceStateDTO() };
            _serviceClientMock.Setup(s => s.GetAvailableStatesAsync(It.IsAny<string>()))
                .ReturnsAsync(states);
            var result = await _controller.GetAvailableStatesInfo("POL123");
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(states, okResult.Value);
        }

        [Fact]
        public async Task GetAvailableStatesInfo_ReturnsInternalServerError_OnException()
        {
            _serviceClientMock.Setup(s => s.GetAvailableStatesAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Test exception"));
            var result = await _controller.GetAvailableStatesInfo("POL123");
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }
        [Fact]
        public async Task GetPolicyStatesInfo_ReturnsOk_WithStates()
        {
            var validatorMock = new Mock<IValidator<GetPolicyStatesQuery>>();
            var states = new List<UWStateDTO> { GetValidUWStateDTO() };
            _serviceClientMock.Setup(s => s.GetPolicyStatesAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApiResponse<IEnumerable<UWStateDTO>>
                {
                    Success = true,
                    Result = states
                });
            var result = await _controller.GetPolicyStatesInfo("POL123", validatorMock.Object);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(states, okResult.Value);
        }

        [Fact]
        public async Task UpsertPolicyStateInfo_ReturnsOk_WhenServiceReturnsSuccess()
        {
            var request = GetValidUpsertRequest();
            _serviceClientMock.Setup(s => s.UpsertPolicyStateAsync(It.IsAny<UpsertPolicyStateCommandRequest>()))
                .ReturnsAsync(new ApiResponse<string>
                {
                    Success = true,
                    Result = "Upserted"
                });
            var result = await _controller.UpsertPolicyStateInfo(request);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Upserted", okResult.Value);
        }
    }
}

