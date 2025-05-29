using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.Domain.V1.EntityModels;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
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

        public UpsertPolicyStateHandlerTests()
        {
            _stateRepositoryMock = new Mock<IStateRepository>();
            _loggerMock = new Mock<ILogger<UpsertPolicyStateHandler>>();
            _mapperMock = new Mock<IMapper>();
            _handler = new UpsertPolicyStateHandler(_stateRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
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
                RiskId = "RISK001"
            };
            //please write all fields in above object
            var uwStateEntity = new UWStateEntity();
            var expectedMessage = "Success";

            _mapperMock.Setup(m => m.Map<UWStateEntity>(command)).Returns(uwStateEntity);
            _stateRepositoryMock.Setup(r => r.UpsertPolicyStateAsync(uwStateEntity)).ReturnsAsync(expectedMessage);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(expectedMessage, result);
            _mapperMock.Verify(m => m.Map<UWStateEntity>(command), Times.Once);
            _stateRepositoryMock.Verify(r => r.UpsertPolicyStateAsync(uwStateEntity), Times.Once);
            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Attempting to upsert policy state")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldLogErrorAndThrow_WhenRepositoryThrows()
        {
            // Arrange
            var command = new UpsertPolicyStateCommand { MgaCode = "MGA2", State = "CA" };
            var uwStateEntity = new UWStateEntity();
            var exception = new Exception("DB error");

            _mapperMock.Setup(m => m.Map<UWStateEntity>(command)).Returns(uwStateEntity);
            _stateRepositoryMock.Setup(r => r.UpsertPolicyStateAsync(uwStateEntity)).ThrowsAsync(exception);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("DB error", ex.Message);
            _loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error upserting policy state")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
}
