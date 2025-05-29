using BHHC.UW.PolicyCenter.Domain.V1.EntityModels;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using Dapper;
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

        [Fact]
        public async Task GetPolicyStatesAsync_ReturnsMappedEntities()
        {
            // Arrange
            var mgaCode = "MGATEST";
            var parameters = new DynamicParameters();
            parameters.Add("@mgacode", mgaCode, DbType.AnsiString, size: 10);
            parameters.Add("@excludeOffPolicy", 'Y', DbType.AnsiString, size: 1);
            var dapperResult = new List<dynamic>
            {
                new { stateName = "California", stateabb = "CA", STBEGIN = DateTime.Today, ST_Tin = "TIN1", RiskID = "R1" },
                new { stateName = "Nevada", stateabb = "NV", STBEGIN = DateTime.Today.AddDays(-1), ST_Tin = "TIN2", RiskID = "R2" }
            };
            var uwStateEntity = new List<UWStateEntity>
            {
                new UWStateEntity
                {
                    MgaCode = mgaCode,
                    State = "CA",
                    Stateabb = "California",
                    STBEGIN = DateTime.Today,
                    ST_Tin = "TIN1",
                    RiskId = "R1"
                },
                new UWStateEntity
                {
                    MgaCode = mgaCode,
                    State = "NV",
                    Stateabb = "Nevada",
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


            // Act
            var result = await _repository.GetPolicyStatesAsync(mgaCode);

            // Assert
            Assert.NotNull(result);
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal("CA", list[0].State);
            Assert.Equal("California", list[0].Stateabb);
            Assert.Equal("NV", list[1].State);
            Assert.Equal("Nevada", list[1].Stateabb);
        }

        [Fact]
        public async Task GetAllAvailableStatesAsync_ReturnsReferenceStates()
        {
            // Arrange
            var mgacode = "MGATEST";
            _dbConnectionMock.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<string>(
                It.Is<string>(q => q.Contains("fn_Policy_GetLob")), It.IsAny<object>(), null, null, null)).ReturnsAsync("WC");

            _dbConnectionMock.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<string>(
                It.Is<string>(q => q.Contains("CASE WHEN @LOB")), It.IsAny<object>(), null, null, null)).ReturnsAsync("C");

            _dbConnectionMock.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<string>(
                It.Is<string>(q => q.Contains("fn_uw_op_getTransactionMGACode")), It.IsAny<object>(), null, null, null)).ReturnsAsync("TXN1");

            _dbConnectionMock.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<(string, string, string)>(
                It.Is<string>(q => q.Contains("FROM insured")), It.IsAny<object>(), null, null, null))
                .ReturnsAsync(("Carrier1", "Agency1", "01/01/2024"));

            var expectedStates = new List<ReferenceStateDTO>
            {
                new ReferenceStateDTO { State = "CA", StateName = "California" },
                new ReferenceStateDTO { State = "NV", StateName = "Nevada" }
            };

            _dbConnectionMock.SetupDapperAsync(c => c.QueryAsync<ReferenceStateDTO>(
                It.IsAny<string>(), null, null, null, null)).ReturnsAsync(expectedStates);

            // Act
            var result = await _repository.GetAllAvailableStatesAsync(mgacode);

            // Assert
            Assert.NotNull(result);
            var list = result.ToList();
            Assert.Equal(2, list.Count);
            Assert.Equal("CA", list[0].State);
            Assert.Equal("Nevada", list[1].StateName);
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
            var result = await _repository.UpsertPolicyStateAsync(uwStateEntity);

            // Assert
            Assert.Null(result);
        }
    }
}
