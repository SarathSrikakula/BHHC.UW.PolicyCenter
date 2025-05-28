using AutoMapper;
using BHHC.UW.PolicyCenter.API.PolicyCenterException;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.API.V1.Application.Queries;
using BHHC.UW.PolicyCenter.API.V1.Controllers;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static BHHC.UW.PolicyCenter.API.PolicyCenterException.HandledException;

namespace BHHC.UW.PolicyCenter.API.Tests.V1.Controllers
{
    public class PolicyStatesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly PolicyStatesController _controller;

        public PolicyStatesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _controller = new PolicyStatesController(_mediatorMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetPolicyStates_ReturnsOk_WithStates()
        {
            // Arrange
            var policyId = "POL123";
            var states = new List<UWStateDTO> { new UWStateDTO() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPolicyStatesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(states);

            // Act
            var result = await _controller.GetPolicyStates(policyId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(states, okResult.Value);
        }

        [Fact]
        public async Task GetPolicyStates_ReturnsBadRequest_OnBusinessLogicException()
        {
            // Arrange
            var policyId = "POL123";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPolicyStatesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessLogicException("Business error"));

            // Act
            var result = await _controller.GetPolicyStates(policyId);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<WebApiExceptionResponseModel>(badRequest.Value);
            Assert.Equal(nameof(BusinessLogicException), response.ExceptionType);
        }

        [Fact]
        public async Task GetPolicyStates_ReturnsInternalServerError_OnHandledException()
        {
            // Arrange
            var policyId = "POL123";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPolicyStatesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HandledException("Handled error"));

            // Act
            var result = await _controller.GetPolicyStates(policyId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            var response = Assert.IsType<WebApiExceptionResponseModel>(objectResult.Value);
            Assert.Equal(nameof(HandledException), response.ExceptionType);
        }

        [Fact]
        public async Task GetPolicyStates_ReturnsInternalServerError_OnGeneralException()
        {
            // Arrange
            var policyId = "POL123";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPolicyStatesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("General error"));

            // Act
            var result = await _controller.GetPolicyStates(policyId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            var response = Assert.IsType<WebApiExceptionResponseModel>(objectResult.Value);
            Assert.Equal("Exception", response.ExceptionType);
        }

        [Fact]
        public async Task UpsertPolicyState_ReturnsOk_OnSuccess()
        {
            // Arrange
            var request = new UpsertPolicyStateCommandRequest();
            var command = new UpsertPolicyStateCommand();
            var responseMessage = "Success";
            _mapperMock.Setup(m => m.Map<UpsertPolicyStateCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>())).ReturnsAsync(responseMessage);

            // Act
            var result = await _controller.UpsertPolicyState(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(responseMessage, okResult.Value);
        }

        [Fact]
        public async Task UpsertPolicyState_ReturnsBadRequest_OnInvalidModel()
        {
            // Arrange
            var request = new UpsertPolicyStateCommandRequest();
            _controller.ModelState.AddModelError("Test", "Invalid");

            // Act
            var result = await _controller.UpsertPolicyState(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpsertPolicyState_ReturnsBadRequest_OnBusinessLogicException()
        {
            // Arrange
            var request = new UpsertPolicyStateCommandRequest();
            var command = new UpsertPolicyStateCommand();
            _mapperMock.Setup(m => m.Map<UpsertPolicyStateCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessLogicException("Business error"));

            // Act
            var result = await _controller.UpsertPolicyState(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<WebApiExceptionResponseModel>(badRequest.Value);
            Assert.Equal(nameof(BusinessLogicException), response.ExceptionType);
        }

        [Fact]
        public async Task UpsertPolicyState_ReturnsInternalServerError_OnHandledException()
        {
            // Arrange
            var request = new UpsertPolicyStateCommandRequest();
            var command = new UpsertPolicyStateCommand();
            _mapperMock.Setup(m => m.Map<UpsertPolicyStateCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HandledException("Handled error"));

            // Act
            var result = await _controller.UpsertPolicyState(request);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            var response = Assert.IsType<WebApiExceptionResponseModel>(objectResult.Value);
            Assert.Equal(nameof(HandledException), response.ExceptionType);
        }

        [Fact]
        public async Task UpsertPolicyState_ReturnsInternalServerError_OnGeneralException()
        {
            // Arrange
            var request = new UpsertPolicyStateCommandRequest();
            var command = new UpsertPolicyStateCommand();
            _mapperMock.Setup(m => m.Map<UpsertPolicyStateCommand>(request)).Returns(command);
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("General error"));

            // Act
            var result = await _controller.UpsertPolicyState(request);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            var response = Assert.IsType<WebApiExceptionResponseModel>(objectResult.Value);
            Assert.Equal("Exception", response.ExceptionType);
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsOk_WithStates()
        {
            // Arrange
            var mgacode = "MGA123";
            var states = new List<ReferenceStateDTO> { new ReferenceStateDTO() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableStatesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(states);

            // Act
            var result = await _controller.GetAvailableStates(mgacode);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(states, okResult.Value);
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsBadRequest_OnBusinessLogicException()
        {
            // Arrange
            var mgacode = "MGA123";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableStatesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new BusinessLogicException("Business error"));

            // Act
            var result = await _controller.GetAvailableStates(mgacode);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<WebApiExceptionResponseModel>(badRequest.Value);
            Assert.Equal(nameof(BusinessLogicException), response.ExceptionType);
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsInternalServerError_OnHandledException()
        {
            // Arrange
            var mgacode = "MGA123";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableStatesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HandledException("Handled error"));

            // Act
            var result = await _controller.GetAvailableStates(mgacode);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            var response = Assert.IsType<WebApiExceptionResponseModel>(objectResult.Value);
            Assert.Equal(nameof(HandledException), response.ExceptionType);
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsInternalServerError_OnGeneralException()
        {
            // Arrange
            var mgacode = "MGA123";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableStatesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("General error"));

            // Act
            var result = await _controller.GetAvailableStates(mgacode);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
            var response = Assert.IsType<WebApiExceptionResponseModel>(objectResult.Value);
            Assert.Equal("Exception", response.ExceptionType);
        }
    }
}
