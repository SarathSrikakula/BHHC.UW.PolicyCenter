using AutoMapper;
using BHHC.UW.PolicyCenter.API.PolicyCenterException;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.API.V1.Application.Models;
using BHHC.UW.PolicyCenter.API.V1.Application.Queries;
using BHHC.UW.PolicyCenter.API.V1.Controllers;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using DigitalPlatform.Errors.Models.Extension;
using DigitalPlatform.Errors.Models.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly Mock<ILogger<PolicyStatesController>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly PolicyStatesController _controller;

        public PolicyStatesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PolicyStatesController>>();
            _configurationMock = new Mock<IConfiguration>();
            _controller = new PolicyStatesController(
                _mediatorMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _configurationMock.Object
            );
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsOk_WhenStatesFound()
        {
            // Arrange
            var mgacode = "MGA123";
            var states = new List<ReferenceStateDTO> { new ReferenceStateDTO { State = "CA", StateName = "California" } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableStatesQuery>(), default)).ReturnsAsync(states);

            // Act
            var result = await _controller.GetAvailableStates(mgacode);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<ApiResponse<IEnumerable<ReferenceStateDTO>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(states, response.Result);
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsBadRequest_WhenMgaCodeIsMissing()
        {
            // Act
            var result = await _controller.GetAvailableStates("");

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsAssignableFrom<ApiResponse<bool>>(badRequest.Value);
            Assert.False(response.Success);
            Assert.NotNull(response.Exception);
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsOkWithError_WhenStatesNull()
        {
            // Arrange
            var mgacode = "MGA123";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableStatesQuery>(), default)).ReturnsAsync((IEnumerable<ReferenceStateDTO>)null);
            _loggerMock.Setup(l => l.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
            ));
            _loggerMock.Setup(l => l.LogCustomError(
                It.IsAny<CustomError>(),
                It.IsAny<LogLevel>(),
                It.IsAny<Exception>(),
                It.IsAny<string>()
            )).Returns("mocked error message");

            // Act
            var result = await _controller.GetAvailableStates(mgacode);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<ApiResponse<ReferenceStateDTO>>(okResult.Value);
            Assert.False(response.Success);
            Assert.NotNull(response.Exception);
        }

        [Fact]
        public async Task GetAvailableStates_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var mgacode = "MGA123";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAvailableStatesQuery>(), default)).ThrowsAsync(new Exception("Test exception"));
            _loggerMock.Setup(l => l.LogCustomError(
                It.IsAny<CustomError>(),
                It.IsAny<LogLevel>(),
                It.IsAny<Exception>(),
                It.IsAny<string>()
            )).Returns("mocked error message");

            // Act
            var result = await _controller.GetAvailableStates(mgacode);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            var response = Assert.IsAssignableFrom<ApiResponse<bool>>(objectResult.Value);
            Assert.False(response.Success);
            Assert.NotNull(response.Exception);
        }
    }
}
