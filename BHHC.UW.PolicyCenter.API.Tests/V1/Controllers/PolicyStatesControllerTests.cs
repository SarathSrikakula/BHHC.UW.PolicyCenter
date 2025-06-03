using AutoMapper;
using BHHC.UW.PolicyCenter.API.PolicyCenterException;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.API.V1.Application.Models;
using BHHC.UW.PolicyCenter.API.V1.Application.Queries;
using BHHC.UW.PolicyCenter.API.V1.Controllers;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static BHHC.UW.PolicyCenter.API.PolicyCenterException.HandledException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace BHHC.UW.PolicyCenter.API.Tests.V1.Controllers
{
    public class PolicyStatesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<ILogger<PolicyStatesController>> _loggerMock;

        public PolicyStatesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _configMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<PolicyStatesController>>();
        }

        [Fact]
        public async Task GetPolicyStates_ReturnsValidationError_WhenPolicyIdIsEmpty()
        {
            var validator = new Mock<IValidator<GetPolicyStatesQuery>>();
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object, _loggerMock.Object, _configMock.Object);

            var result = await controller.GetPolicyStates("", validator.Object);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<PolicyAssignedStatesDTO1>>(okResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Invalid PolicyId", response.ValidationMessage);
        }

        [Fact]
        public async Task GetPolicyStates_ReturnsValidationError_WhenValidationFails()
        {
            var validator = new Mock<IValidator<GetPolicyStatesQuery>>();
            validator.Setup(v => v.ValidateAsync(It.IsAny<GetPolicyStatesQuery>(), default))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("MgaCode", "Error") }));
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object, _loggerMock.Object, _configMock.Object);

            var result = await controller.GetPolicyStates("123", validator.Object);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.False(response.Success);
            Assert.Contains("Error", response.ValidationMessage);
        }

        [Fact]
        public async Task GetPolicyStates_ReturnsStates_WhenValid()
        {
            var validator = new Mock<IValidator<GetPolicyStatesQuery>>();
            validator.Setup(v => v.ValidateAsync(It.IsAny<GetPolicyStatesQuery>(), default))
                .ReturnsAsync(new ValidationResult());
            var states = new List<PolicyAssignedStatesDTO1> { new PolicyAssignedStatesDTO1 { PolicyCode = "123", State = "CA" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPolicyStatesQuery>(), default)).ReturnsAsync(states);
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object, _loggerMock.Object, _configMock.Object);

            var result = await controller.GetPolicyStates("123", validator.Object);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<PolicyAssignedStatesDTO1>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(states, response.Result);
        }

        [Fact]
        public async Task UpsertPolicyState_ReturnsValidationError_WhenValidationFails()
        {
            var validator = new Mock<IValidator<UpsertPolicyStateCommandRequest>>();
            validator.Setup(v => v.ValidateAsync(It.IsAny<UpsertPolicyStateCommandRequest>(), default))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("MgaCode", "Error") }));
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object, _loggerMock.Object, _configMock.Object);

            var result = await controller.UpsertPolicyState(new UpsertPolicyStateCommandRequest(), validator.Object);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.False(response.Success);
            Assert.Contains("Error", response.ValidationMessage);
        }

        [Fact]
        public async Task UpsertPolicyState_ReturnsSuccess_WhenValid()
        {
            var validator = new Mock<IValidator<UpsertPolicyStateCommandRequest>>();
            validator.Setup(v => v.ValidateAsync(It.IsAny<UpsertPolicyStateCommandRequest>(), default))
                .ReturnsAsync(new ValidationResult());
            var command = new UpsertPolicyStateCommand();
            _mapperMock.Setup(m => m.Map<UpsertPolicyStateCommand>(It.IsAny<UpsertPolicyStateCommandRequest>())).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, default)).ReturnsAsync("Success");
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object, _loggerMock.Object, _configMock.Object);

            var result = await controller.UpsertPolicyState(new UpsertPolicyStateCommandRequest(), validator.Object);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Success", response.Result);
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsBadRequest_WhenMgaCodeIsEmpty()
        {
            var validator = new Mock<IValidator<GetAvailableStatesQuery>>();
            validator.Setup(v => v.ValidateAsync(It.IsAny<GetAvailableStatesQuery>(), default))
                .ReturnsAsync(new ValidationResult());
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object, _loggerMock.Object, _configMock.Object);

            var result = await controller.GetAvailableStates("", validator.Object);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<WebApiExceptionResponseModel>(badRequest.Value);
            Assert.Equal("MgaCode is required to retrieve available states.", response.ExceptionMessage);
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsStates_WhenValid()
        {
            var validator = new Mock<IValidator<GetAvailableStatesQuery>>();
            validator.Setup(v => v.ValidateAsync(It.IsAny<GetAvailableStatesQuery>(), default))
                .ReturnsAsync(new ValidationResult());
            var states = new List<PolicyAvailableStatesDTO1> { new PolicyAvailableStatesDTO1 { State = "CA", StateName = "California" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableStatesQuery>(), default)).ReturnsAsync(states);
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object, _loggerMock.Object, _configMock.Object);

            var result = await controller.GetAvailableStates("123", validator.Object);

            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpsertPolicyState_ReturnsException_WhenMediatorThrows()
        {
            var validator = new Mock<IValidator<UpsertPolicyStateCommandRequest>>();
            validator.Setup(v => v.ValidateAsync(It.IsAny<UpsertPolicyStateCommandRequest>(), default))
                .ReturnsAsync(new ValidationResult());
            var command = new UpsertPolicyStateCommand();
            _mapperMock.Setup(m => m.Map<UpsertPolicyStateCommand>(It.IsAny<UpsertPolicyStateCommandRequest>())).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, default)).ThrowsAsync(new Exception("Unexpected error"));
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object, _loggerMock.Object, _configMock.Object);

            var result = await controller.UpsertPolicyState(new UpsertPolicyStateCommandRequest(), validator.Object);

            var okResult = Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public async Task UpsertPolicyState_ReturnsValidationError_WhenRequestIsNull()
        {
            var validator = new Mock<IValidator<UpsertPolicyStateCommandRequest>>();
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object, _loggerMock.Object, _configMock.Object);

            var result = await controller.UpsertPolicyState(null, validator.Object);

            var okResult = Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsNotFound_WhenResultIsNull()
        {
            var validator = new Mock<IValidator<GetAvailableStatesQuery>>();
            validator.Setup(v => v.ValidateAsync(It.IsAny<GetAvailableStatesQuery>(), default))
                .ReturnsAsync(new ValidationResult());
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableStatesQuery>(), default)).ReturnsAsync((IEnumerable<PolicyAvailableStatesDTO1>)null);
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object, _loggerMock.Object, _configMock.Object);

            var result = await controller.GetAvailableStates("123", validator.Object);

            var notFoundResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpsertPolicyState_UsesUpsertPolicyStateCommandRequest_WithValidProperties()
        {
            var validator = new Mock<IValidator<UpsertPolicyStateCommandRequest>>();
            validator.Setup(v => v.ValidateAsync(It.IsAny<UpsertPolicyStateCommandRequest>(), default))
                .ReturnsAsync(new ValidationResult());

            var request = new UpsertPolicyStateCommandRequest
            {
                MgaCode = "MGA123",
                StateBeginDate = new DateTime(2024, 1, 1),
                StateTin = "TIN456",
                RiskId = "RISK789",
                State = "CA"
            };

            var command = new UpsertPolicyStateCommand
            {
                MgaCode = request.MgaCode,
                StateBeginDate = request.StateBeginDate,
                StateTin = request.StateTin,
                RiskId = request.RiskId,
                State = request.State
            };

            _mapperMock.Setup(m => m.Map<UpsertPolicyStateCommand>(It.IsAny<UpsertPolicyStateCommandRequest>())).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, default)).ReturnsAsync("Success");

            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object, _loggerMock.Object, _configMock.Object);

            var result = await controller.UpsertPolicyState(request, validator.Object);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Success", response.Result);
        }
    }
}
