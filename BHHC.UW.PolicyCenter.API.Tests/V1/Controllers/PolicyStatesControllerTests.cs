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

        public PolicyStatesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetPolicyStates_ReturnsValidationError_WhenPolicyIdIsEmpty()
        {
            var validator = new Mock<IValidator<GetPolicyStatesQuery>>();
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object);

            var result = await controller.GetPolicyStates("", validator.Object);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<UWStateDTO>>(okResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Invalid PolicyId", response.ValidationMessage);
        }

        [Fact]
        public async Task GetPolicyStates_ReturnsValidationError_WhenValidationFails()
        {
            var validator = new Mock<IValidator<GetPolicyStatesQuery>>();
            validator.Setup(v => v.ValidateAsync(It.IsAny<GetPolicyStatesQuery>(), default))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("MgaCode", "Error") }));
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object);

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
            var states = new List<UWStateDTO> { new UWStateDTO { MgaCode = "123", State = "CA" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPolicyStatesQuery>(), default)).ReturnsAsync(states);
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object);

            var result = await controller.GetPolicyStates("123", validator.Object);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<UWStateDTO>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(states, response.Result);
        }

        [Fact]
        public async Task UpsertPolicyState_ReturnsValidationError_WhenValidationFails()
        {
            var validator = new Mock<IValidator<UpsertPolicyStateCommandRequest>>();
            validator.Setup(v => v.ValidateAsync(It.IsAny<UpsertPolicyStateCommandRequest>(), default))
                .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("MgaCode", "Error") }));
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object);

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
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object);

            var result = await controller.UpsertPolicyState(new UpsertPolicyStateCommandRequest(), validator.Object);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Success", response.Result);
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsBadRequest_WhenMgaCodeIsEmpty()
        {
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object);

            var result = await controller.GetAvailableStates("");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<WebApiExceptionResponseModel>(badRequest.Value);
            Assert.Equal("MgaCode is required to retrieve available states.", response.ExceptionMessage);
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsStates_WhenValid()
        {
            var states = new List<ReferenceStateDTO> { new ReferenceStateDTO { State = "CA", StateName = "California" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableStatesQuery>(), default)).ReturnsAsync(states);
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object);

            var result = await controller.GetAvailableStates("123");

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(states, okResult.Value);
        }

        [Fact]
        public async Task GetAvailableStates_WithValidator_ReturnsStates_WhenValid()
        {
            var validator = new Mock<IValidator<GetAvailableStatesQuery>>();
            validator.Setup(v => v.ValidateAsync(It.IsAny<GetAvailableStatesQuery>(), default))
                .ReturnsAsync(new ValidationResult());
            var states = new List<ReferenceStateDTO> { new ReferenceStateDTO { State = "CA", StateName = "California" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableStatesQuery>(), default)).ReturnsAsync(states);
            var controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object);

            var result = await controller.GetAvailableStates("123", "123", validator.Object);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ReferenceStateDTO>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(states, response.Result);
        }
    }
}
