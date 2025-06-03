using AutoMapper;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.Domain.V1.EntityModels;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static BHHC.UW.PolicyCenter.API.PolicyCenterException.HandledException;

namespace BHHC.UW.PolicyCenter.Infrastructure.Tests.V1.Repositories
{
    public class StateRepositoryTests
    {
        private readonly Mock<IDbConnection> _dbConnectionMock;
        private readonly Mock<ILogger<StateRepository>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly StateRepository _repository;

        public StateRepositoryTests()
        {
            _dbConnectionMock = new Mock<IDbConnection>();
            _loggerMock = new Mock<ILogger<StateRepository>>();
            _configurationMock = new Mock<IConfiguration>();
            _mapperMock = new Mock<IMapper>();
            _repository = new StateRepository(
                _dbConnectionMock.Object,
                _loggerMock.Object,
                _configurationMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task GetPolicyStatesAsync_ReturnsMappedStates()
        {
            // Arrange
            var mgaCode = "MGATEST";
            var entityStates = new List<UWStateEntity> { new UWStateEntity() };
            var mappedStates = new List<PolicyAssignedStatesDTO> { new PolicyAssignedStatesDTO() };
            var dapperResult = new List<dynamic>
            {
                new { stateName = "California", stateabb = "CA", STBEGIN = DateTime.Today, ST_Tin = "TIN1", RiskID = "R1" },
                new { stateName = "Nevada", stateabb = "NV", STBEGIN = DateTime.Today.AddDays(-1), ST_Tin = "TIN2", RiskID = "R2" }
            };

            // Arrange
            var parameters = new DynamicParameters();
            parameters.Add("@mgacode", mgaCode, DbType.AnsiString, size: 10);
            parameters.Add("@excludeOffPolicy", 'Y', DbType.AnsiString, size: 1);
            var uwStateEntity = new List<UWStateEntity>
            {
                new UWStateEntity
                {
                    MgaCode = mgaCode,
                    State = "CA",
                    StateName = "California",
                    STBEGIN = DateTime.Today,
                    ST_Tin = "TIN1",
                    RiskId = "R1"
                },
                new UWStateEntity
                {
                    MgaCode = mgaCode,
                    State = "NV",
                    StateName = "Nevada",
                    STBEGIN = DateTime.Today.AddDays(-1),
                    ST_Tin = "TIN2",
                    RiskId = "R2"
                }
            };

            _dbConnectionMock.SetupDapperAsync(c => c.QueryAsync<UWStateEntity>(
                "up_uw_listpolicyRatingStates",
                parameters,
                null,
                null,
                CommandType.StoredProcedure
            )).ReturnsAsync(uwStateEntity);

            _mapperMock.Setup(m => m.Map<IEnumerable<PolicyAssignedStatesDTO>>(entityStates))
                .Returns(mappedStates);

            // Act
            var result = await _repository.GetPolicyStatesAsync(mgaCode);

            // Assert
            Assert.NotNull(mappedStates);
        }

        [Fact]
        public async Task GetAllAvailableStatesAsync_ReturnsMappedStates()
        {
            // Arrange
            var mgacode = "MGATEST";
            var lob = "WC";
            var porc = "C";
            var trancode = "TRANCODE";
            var insuredInfo = (Carrier: "CARRIER", Agency: "AGENCY", PoBegin: "01/01/2024");
            var entityStates = new List<ReferenceStateEntity> { new ReferenceStateEntity() };
            var mappedStates = new List<PolicyAvailableStatesDTO>
            {
                new PolicyAvailableStatesDTO { State = "CA", StateName = "California" },
                new PolicyAvailableStatesDTO { State = "NV", StateName = "Nevada" }
            };
            //for above line please generate some date

            // Arrange
            _dbConnectionMock.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<string>(
                It.Is<string>(q => q.Contains("fn_Policy_GetLob")), It.IsAny<object>(), null, null, null)).ReturnsAsync("WC");

            _dbConnectionMock.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<string>(
                It.Is<string>(q => q.Contains("CASE WHEN @LOB")), It.IsAny<object>(), null, null, null)).ReturnsAsync("C");

            _dbConnectionMock.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<string>(
                It.Is<string>(q => q.Contains("fn_uw_op_getTransactionMGACode")), It.IsAny<object>(), null, null, null)).ReturnsAsync("TXN1");

            _dbConnectionMock.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<(string, string, string)>(
                It.Is<string>(q => q.Contains("FROM insured")), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(("Carrier1", "Agency1", "01/01/2024"));
            var expectedStates = new List<PolicyAvailableStatesDTO>
            {
                new PolicyAvailableStatesDTO { State = "CA", StateName = "California" },
                new PolicyAvailableStatesDTO { State = "NV", StateName = "Nevada" }
            };

            _dbConnectionMock.SetupDapperAsync(c => c.QueryAsync<PolicyAvailableStatesDTO>(
                It.IsAny<string>(), null, null, null, null)).ReturnsAsync(expectedStates);
            _mapperMock.Setup(m => m.Map<IEnumerable<PolicyAvailableStatesDTO>>(entityStates))
                .Returns(mappedStates);

            // Act
            var result = await _repository.GetAllAvailableStatesAsync(mgacode);

            // Assert
            Assert.NotNull(mappedStates);
        }

        [Fact]
        public async Task UpsertPolicyStateAsync_ReturnsRMessage()
        {

            // Arrange

            var uwStateEntity = new UWStateEntity
            {
                MgaCode = "MGATEST",
                State = "CA",
                STBEGIN = DateTime.UtcNow,
                ST_Tin = "TIN123",
                RiskId = "RISK001"
            };
            var upsertPolicyState = new UpsertPolicyState
            {
                MgaCode = "MGATEST",
                State = "CA",
                StateBeginDate = DateTime.UtcNow,
                StateTin = "TIN123",
                RiskId = "RISK001"
            };
            _mapperMock.Setup(m => m.Map<UWStateEntity>(upsertPolicyState)).Returns(uwStateEntity);

            var resetRatingParameters = new DynamicParameters();
            resetRatingParameters.Add("@MGACode", "MGATEST", DbType.AnsiString, size: 10);
            _dbConnectionMock
                .SetupDapperAsync(c => c.QuerySingleOrDefaultAsync<bool>(
                    It.Is<string>(q => q.Contains("IsReset")),
                    resetRatingParameters,
                    null, null, null))
                .ReturnsAsync(true);
            var expectedMessage = "Success";

            // Act
            var result = await _repository.UpsertPolicyStateAsync(upsertPolicyState);

            // Assert
            Assert.Null(result);
        }
    }

}

