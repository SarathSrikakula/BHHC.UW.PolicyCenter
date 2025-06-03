using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.Domain.V1.EntityModels;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
namespace BHHC.UW.PolicyCenter.API.Tests.V1.Application.Commands
{
    public class UpsertPolicyStateHandlerTests
    {
        private readonly Mock<IStateRepository> _stateRepositoryMock;
        private readonly Mock<ILogger<UpsertPolicyStateHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UpsertPolicyStateHandler _handler;
        private readonly Mock<IConfiguration> _configurationMock;

        public UpsertPolicyStateHandlerTests()
        {
            _stateRepositoryMock = new Mock<IStateRepository>();
            _loggerMock = new Mock<ILogger<UpsertPolicyStateHandler>>();
            _mapperMock = new Mock<IMapper>();
            _configurationMock = new Mock<IConfiguration>();
            _handler = new UpsertPolicyStateHandler(_stateRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnRepositoryMessage_WhenUpsertSucceeds()
        {
            // Arrange
            var command = new UpsertPolicyStateCommand
            {
                MgaCode = "MGA1",
                State = "TX",
                StateBeginDate = new DateTime(2024, 1, 1),
                StateTin = "TIN123",
                RiskId = "RISK1"
            };
            var upsertPolicyState = new UpsertPolicyState
            {
                MgaCode = command.MgaCode,
                State = command.State,
                StateBeginDate = command.StateBeginDate,
                StateTin = command.StateTin,
                RiskId = command.RiskId
            };
            var expectedMessage = "Success";

            _mapperMock.Setup(m => m.Map<UpsertPolicyState>(command)).Returns(upsertPolicyState);
            _stateRepositoryMock.Setup(r => r.UpsertPolicyStateAsync(upsertPolicyState)).ReturnsAsync(expectedMessage);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(expectedMessage, result);
            _mapperMock.Verify(m => m.Map<UpsertPolicyState>(command), Times.Once);
            _stateRepositoryMock.Verify(r => r.UpsertPolicyStateAsync(upsertPolicyState), Times.Once);
        }
    }
}
