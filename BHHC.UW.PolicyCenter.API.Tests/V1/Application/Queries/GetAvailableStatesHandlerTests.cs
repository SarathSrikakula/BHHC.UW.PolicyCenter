using AutoMapper;
using BHHC.UW.PolicyCenter.API.V1.Application.Queries;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BHHC.UW.PolicyCenter.API.PolicyCenterException.HandledException;

namespace BHHC.UW.PolicyCenter.API.Tests.V1.Application.Queries
{
    public class GetAvailableStatesHandlerTests
    {
        private readonly Mock<IStateRepository> _stateRepositoryMock;
        private readonly Mock<ILogger<GetAvailableStatesHandler>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAvailableStatesHandler _handler;

        public GetAvailableStatesHandlerTests()
        {
            _stateRepositoryMock = new Mock<IStateRepository>();
            _loggerMock = new Mock<ILogger<GetAvailableStatesHandler>>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAvailableStatesHandler(_stateRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ValidMgaCode_ReturnsMappedReferenceStates()
        {
            // Arrange
            var mgaCode = "TX";
            var query = new GetAvailableStatesQuery { MgaCode = mgaCode };
            var entityStates = new List<ReferenceStateDTO>
            {
                new ReferenceStateDTO { State = "TX", StateName = "Texas" },
                new ReferenceStateDTO { State = "CA", StateName = "California" }
            };

            var dtoStates = new List<ReferenceStateDTO>
            {
                new ReferenceStateDTO { State = "TX", StateName = "Texas" },
                new ReferenceStateDTO { State = "CA", StateName = "California" }
            };

            _stateRepositoryMock.Setup(r => r.GetAllAvailableStatesAsync(mgaCode))
                .ReturnsAsync(entityStates.AsEnumerable());
            _mapperMock.Setup(m => m.Map<IEnumerable<ReferenceStateDTO>>(entityStates))
                .Returns(dtoStates);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(dtoStates, result);
            _stateRepositoryMock.Verify(r => r.GetAllAvailableStatesAsync(mgaCode), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<ReferenceStateDTO>>(entityStates), Times.Once);
        }

        [Fact]
        public async Task Handle_MgaCodeIsNull_ThrowsBusinessLogicException()
        {
            // Arrange
            var query = new GetAvailableStatesQuery { MgaCode = null };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessLogicException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_RepositoryThrowsException_LogsErrorAndThrows()
        {
            // Arrange
            var mgaCode = "CA";
            var query = new GetAvailableStatesQuery { MgaCode = mgaCode };
            _stateRepositoryMock.Setup(r => r.GetAllAvailableStatesAsync(mgaCode))
                .ThrowsAsync(new Exception("DB error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ),
                Times.Once
            );
        }
    }
}
