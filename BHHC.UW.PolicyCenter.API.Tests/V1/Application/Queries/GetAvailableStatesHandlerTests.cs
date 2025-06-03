using AutoMapper;
using BHHC.UW.PolicyCenter.API.V1.Application.Queries;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using Microsoft.Extensions.Configuration;
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
        private readonly Mock<IConfiguration> _configurationMock;

        public GetAvailableStatesHandlerTests()
        {
            _stateRepositoryMock = new Mock<IStateRepository>();
            _loggerMock = new Mock<ILogger<GetAvailableStatesHandler>>();
            _mapperMock = new Mock<IMapper>();
            _configurationMock = new Mock<IConfiguration>();
            _handler = new GetAvailableStatesHandler(_stateRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task Handle_ValidMgaCode_ReturnsMappedReferenceStates()
        {
            // Arrange
            var mgaCode = "TX";
            var query = new GetAvailableStatesQuery { MgaCode = mgaCode };
            var entityStates = new List<PolicyAvailableStatesDTO>
            {
                new PolicyAvailableStatesDTO { State = "TX", StateName = "Texas" },
                new PolicyAvailableStatesDTO { State = "CA", StateName = "California" }
            };

            var dtoStates1 = new List<PolicyAvailableStatesDTO1>
            {
                new PolicyAvailableStatesDTO1 { State = "TX", StateName = "Texas" },
                new PolicyAvailableStatesDTO1 { State = "CA", StateName = "California" }
            };

            _stateRepositoryMock.Setup(r => r.GetAllAvailableStatesAsync(mgaCode))
                .ReturnsAsync(entityStates.AsEnumerable());
            _mapperMock.Setup(m => m.Map<IEnumerable<PolicyAvailableStatesDTO1>>(entityStates))
                .Returns(dtoStates1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(dtoStates1, result);
            //_stateRepositoryMock.Verify(r => r.GetAllAvailableStatesAsync(mgaCode), Times.Once);
            //_mapperMock.Verify(m => m.Map<IEnumerable<PolicyAvailableStatesDTO>>(entityStates), Times.Once);
        }
    }
}
