using AutoMapper;
using BHHC.UW.PolicyCenter.Domain.V1.EntityModels;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static BHHC.UW.PolicyCenter.API.PolicyCenterException.HandledException;

namespace BHHC.UW.PolicyCenter.API.V1.Application.Queries
{
    public class GetPolicyStatesQueryTests : IRequest<IEnumerable<UWStateDTO>>
    {
        public class GetPolicyStatesHandlerTests
        {
            private readonly Mock<IStateRepository> _stateRepositoryMock;
            private readonly Mock<ILogger<GetPolicyStatesHandler>> _loggerMock;
            private readonly Mock<IMapper> _mapperMock;
            private readonly GetPolicyStatesHandler _handler;

            public GetPolicyStatesHandlerTests()
            {
                _stateRepositoryMock = new Mock<IStateRepository>();
                _loggerMock = new Mock<ILogger<GetPolicyStatesHandler>>();
                _mapperMock = new Mock<IMapper>();
                _handler = new GetPolicyStatesHandler(_stateRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
            }

            [Fact]
            public async Task Handle_ValidMgaCode_ReturnsMappedStates()
            {
                // Arrange
                var mgaCode = "TX";
                var query = new GetPolicyStatesQuery { MgaCode = mgaCode };
                var entityStates = new List<UWStateEntity>
                    {
                        new UWStateEntity { MgaCode = "TX", State = "Texas", Stateabb = "Texas" },
                        new UWStateEntity { MgaCode = "TX", State = "California", Stateabb = "California" }
                    };

                var dtoStates = new List<UWStateDTO>
                    {
                        new UWStateDTO
                        {
                            MgaCode = "TX",
                            State = "Texas",
                            StateName = "Texas",
                            BeginDate = new DateTime(2020, 1, 1),
                            StateTin = "TIN123",
                            RiskId = "RISK001"
                        },
                        new UWStateDTO
                        {
                            MgaCode = "TX",
                            State = "California",
                            StateName = "California",
                            BeginDate = new DateTime(2021, 2, 2),
                            StateTin = "TIN456",
                            RiskId = "RISK002"
                        }
                    };

                _stateRepositoryMock.Setup(r => r.GetPolicyStatesAsync(mgaCode))
                    .ReturnsAsync(entityStates.AsEnumerable());
                _mapperMock.Setup(m => m.Map<IEnumerable<UWStateDTO>>(entityStates))
                    .Returns(dtoStates);

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
                Assert.Equal(dtoStates, result);
                _stateRepositoryMock.Verify(r => r.GetPolicyStatesAsync(mgaCode), Times.Once);
                _mapperMock.Verify(m => m.Map<IEnumerable<UWStateDTO>>(entityStates), Times.Once);
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());

                var resultList = result.ToList();
                Assert.Equal("TX", resultList[0].MgaCode);
                Assert.Equal("Texas", resultList[0].State);
                Assert.Equal("Texas", resultList[0].StateName);
                Assert.Equal(new DateTime(2020, 1, 1), resultList[0].BeginDate);
                Assert.Equal("TIN123", resultList[0].StateTin);
                Assert.Equal("RISK001", resultList[0].RiskId);

                Assert.Equal("TX", resultList[1].MgaCode);
                Assert.Equal("California", resultList[1].State);
                Assert.Equal("California", resultList[1].StateName);
                Assert.Equal(new DateTime(2021, 2, 2), resultList[1].BeginDate);
                Assert.Equal("TIN456", resultList[1].StateTin);
                Assert.Equal("RISK002", resultList[1].RiskId);

            }

            [Fact]
            public async Task Handle_MgaCodeIsNull_ThrowsBusinessLogicException()
            {
                // Arrange
                var query = new GetPolicyStatesQuery { MgaCode = null };

                // Act & Assert
                await Assert.ThrowsAsync<BusinessLogicException>(() => _handler.Handle(query, CancellationToken.None));
            }

            [Fact]
            public async Task Handle_RepositoryThrowsException_LogsErrorAndThrows()
            {
                // Arrange
                var mgaCode = "CA";
                var query = new GetPolicyStatesQuery { MgaCode = mgaCode };
                _stateRepositoryMock.Setup(r => r.GetPolicyStatesAsync(mgaCode))
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

}
