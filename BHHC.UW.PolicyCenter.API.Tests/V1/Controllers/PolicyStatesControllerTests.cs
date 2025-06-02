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

        public PolicyStatesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PolicyStatesController>>();
            _configurationMock = new Mock<IConfiguration>();
        }

        [Fact]
        public async Task UpsertPolicyState_BusinessLogicException_LogsCustomError()
        {
            // Arrange
            var controller = new PolicyStatesController(
                _mediatorMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _configurationMock.Object
            );

            var request = new UpsertPolicyStateCommandRequest
            {
                MgaCode = "MGA1",
                State = "CA"
            };

            var command = new UpsertPolicyStateCommand();
            _mapperMock.Setup(m => m.Map<UpsertPolicyStateCommand>(It.IsAny<UpsertPolicyStateCommandRequest>()))
                .Returns(command);

            var exception = new BusinessLogicException("Business error");
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpsertPolicyStateCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            // Act
            var result = await controller.UpsertPolicyState(request);

            // Assert
            // Instead of verifying the extension method, verify that ILogger.Log was called with expected parameters
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("4001") && v.ToString().Contains("Business")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse<bool>>(badRequestResult.Value);
            Assert.False(apiResponse.Success);
            Assert.NotNull(apiResponse.Exception);
        }
    }
}
