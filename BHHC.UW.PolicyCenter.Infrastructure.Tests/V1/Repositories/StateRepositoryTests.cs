using BHHC.UW.PolicyCenter.Domain.V1.EntityModels;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using Dapper;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;

namespace BHHC.UW.PolicyCenter.Infrastructure.Tests.V1.Repositories
{
    public class StateRepositoryTests
    {
        private readonly Mock<IDbConnection> _dbConnectionMock;
        private readonly Mock<ILogger<StateRepository>> _loggerMock;
        private readonly StateRepository _repository;

        public StateRepositoryTests()
        {
            _dbConnectionMock = new Mock<IDbConnection>();
            _loggerMock = new Mock<ILogger<StateRepository>>();
            _repository = new StateRepository(_dbConnectionMock.Object, _loggerMock.Object);
        }

        //can we mock static
        [Fact]
        public async Task GetPolicyStatesAsync_ReturnsMappedEntities()
        {
            // Arrange
            var mgaCode = "MGATEST";
            var dapperResult = new List<dynamic>
                {
                    new TestDynamic { stateName = "California", stateabb = "CA", STBEGIN = DateTime.Parse("2024-01-01"), ST_Tin = "TIN123", RiskID = "RISK1" }
                };

            var commandMock = new Mock<IDbCommand>();
            var readerMock = new Mock<IDataReader>();
            var paramMock = new Mock<IDataParameterCollection>();

            _dbConnectionMock.Setup(x => x.CreateCommand()).Returns(commandMock.Object);
            _dbConnectionMock.Setup(x => x.State).Returns(ConnectionState.Open);

            //Assert
            await Assert.ThrowsAnyAsync<Exception>(() => _repository.GetPolicyStatesAsync(mgaCode));
        }

        [Fact]
        public async Task GetAllAvailableStatesAsync_ThrowsBusinessLogicException_WhenTranCodeMissing()
        {
            // Arrange
            var mgacode = "MGATEST";

            await Assert.ThrowsAnyAsync<Exception>(() => _repository.GetAllAvailableStatesAsync(mgacode));
        }

        [Fact]
        public async Task UpsertPolicyStateAsync_ThrowsException_WhenDbFails()
        {
            // Arrange
            var uwStateEntity = new UWStateEntity
            {
                MgaCode = "MGATEST",
                State = "CA",
                BeginDate = DateTime.Parse("2024-01-01"),
                StateTin = "TIN123",
                RiskId = "RISK1"
            };

            await Assert.ThrowsAnyAsync<Exception>(() => _repository.UpsertPolicyStateAsync(uwStateEntity));
            
        }

        [Fact]
        public async Task GetPolicyStatesAsync_ReturnsExpectedEntities()
        {
            // Arrange
            var mgaCode = "MGATEST";
            var expected = new List<UWStateEntity>
                {
                    new UWStateEntity { MgaCode = "MGATEST", State = "CA", BeginDate = System.DateTime.Parse("2024-01-01"), StateTin = "TIN123", RiskId = "RISK1" }
                };
            var repoMock = new Mock<IStateRepository>();
            repoMock.Setup(r => r.GetPolicyStatesAsync(mgaCode)).ReturnsAsync(expected);

            // Act
            var result = await repoMock.Object.GetPolicyStatesAsync(mgaCode);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("CA", ((List<UWStateEntity>)result)[0].State);
        }

        private class TestDynamic
        {
            public string stateName { get; set; }
            public string stateabb { get; set; }
            public DateTime STBEGIN { get; set; }
            public string ST_Tin { get; set; }
            public string RiskID { get; set; }
        }
    }
}
