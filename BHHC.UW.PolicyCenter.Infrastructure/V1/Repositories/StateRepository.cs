using BHHC.UW.PolicyCenter.API.PolicyCenterException;
using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.Domain.V1.EntityModels;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using Dapper;
using DigitalPlatform.Errors.Models.Extension;
using DigitalPlatform.Errors.Models.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BHHC.UW.PolicyCenter.API.PolicyCenterException.HandledException;

namespace BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories
{
    public class StateRepository : IStateRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<StateRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly AutoMapper.IMapper _mapper; 

        public StateRepository(IDbConnection dbConnection, ILogger<StateRepository> logger, IConfiguration configuration, AutoMapper.IMapper mapper)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); // Initialize _mapper
        }

        public async Task<IEnumerable<PolicyAssignedStatesDTO>> GetPolicyStatesAsync(string mgaCode)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@mgacode", mgaCode, DbType.AnsiString, size: 10);
                parameters.Add("@excludeOffPolicy", 'Y', DbType.AnsiString, size: 1);

                var entityStates = await _dbConnection.QueryAsync<UWStateEntity>( // Fix the missing 'entityStates'
                    "up_uw_listpolicyRatingStates",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return _mapper.Map<IEnumerable<PolicyAssignedStatesDTO>>(entityStates); 
            }
            catch (SqlException ex)
            {
                var errorMessage = _logger.LogCustomError(
                   new CustomError(_configuration, GlobalErrorCategory.Unhandled, "Database error in GetPolicyStatesAsync for MgaCode: {MgaCode}", false),
                   LogLevel.Error,
                   ex,
                   nameof(GetPolicyStatesAsync),
                   null
               );
                throw;
            }
        }
        //can you  write unit test case for above code
        public async Task<IEnumerable<PolicyAvailableStatesDTO>> GetAllAvailableStatesAsync(string mgacode)
        {
            try
            {
                // 1. Get Line of Business
                var lobQuery = "SELECT dbo.fn_Policy_GetLob(@MGACODE) AS val";
                var fnLob = await _dbConnection.QueryFirstOrDefaultAsync<string>(lobQuery, new { MGACODE = mgacode });

                // 2. Get POR_C value (using SQL CASE as in original)
                var porcQuery = "SELECT CASE WHEN @LOB = 'WC' THEN 'C' ELSE '' END AS val";
                var fnPorC = await _dbConnection.QueryFirstOrDefaultAsync<string>(porcQuery, new { LOB = fnLob });

                // 3. Get Transaction Code
                var trancodeQuery = "SELECT dbo.fn_uw_op_getTransactionMGACode(@MGACODE) AS val";
                var fnTranCode = await _dbConnection.QueryFirstOrDefaultAsync<string>(trancodeQuery, new { MGACODE = mgacode });

                if (string.IsNullOrEmpty(fnTranCode))
                    throw new BusinessLogicException("Transaction code not found for the given policy.");

                // 4. Get all insured data in one query
                var insuredQuery = @"
                    SELECT
                        carrier AS Carrier,
                        agency AS Agency,
                        CONVERT(VARCHAR(10), pobegin, 101) AS PoBegin
                    FROM insured
                    WHERE code = @TranCode";

                var insuredInfo = await _dbConnection.QueryFirstOrDefaultAsync<(string Carrier, string Agency, string PoBegin)>(
                    insuredQuery,
                    new { TranCode = fnTranCode });

                if (insuredInfo == default)
                    throw new BusinessLogicException("Insured information not found for the policy's transaction code.");

                // 5. Execute final validation (see original comments)
                var validationQuery = @"
                    SELECT
                        dbo.fn_ValidateStateAppt(
                            s.stateabb,
                            @Agency,
                            @PoBegin,
                            @Carrier,
                            @P_OR_C) AS HasValidateStateAppt
                    FROM xyz; -- Replace 'xyz' with actual table
                ";
                // Example: await _dbConnection.ExecuteAsync(validationQuery, new { Agency = insuredInfo.Agency, PoBegin = insuredInfo.PoBegin, Carrier = insuredInfo.Carrier, P_OR_C = fnPorC });

                var query = "select distinct names, vals from @hasRates";
                var entityStates= await _dbConnection.QueryAsync<ReferenceStateEntity>(query);
                return _mapper.Map<IEnumerable<PolicyAvailableStatesDTO>>(entityStates);
            }
            catch (Exception ex)
            {
                var errorMessage = _logger.LogCustomError(
                   new CustomError(_configuration, GlobalErrorCategory.Unhandled, "An unexpected error occurred in GetAllAvailableStatesAsync for dropdown, MgaCode: {MgaCode}", false),
                   LogLevel.Error,
                   ex,
                   nameof(GetAllAvailableStatesAsync),
                   null
               );
                throw;
            }
        }

        public async Task<string> UpsertPolicyStateAsync(UpsertPolicyState upsertPolicyState)
        {
            try
            {
                _logger.LogInformation("Executing query to retrieve ResetRating for Policy: {MgaCode}", upsertPolicyState.MgaCode);
                var uwStateEntity=_mapper.Map<UWStateEntity>(upsertPolicyState); 
                //please convert the above code to dynamic parameters style
                var resetRatingParameters = new DynamicParameters();
                resetRatingParameters.Add("@MGACode", uwStateEntity.MgaCode, DbType.AnsiString, size: 10);

                var resetRatingQuerySql = @"
                    SELECT CAST(1 AS BIT) AS IsReset;
                ";
                var resetRatingResult = await _dbConnection.QuerySingleOrDefaultAsync<bool>(
                    resetRatingQuerySql,
                    resetRatingParameters
                );
                var parameters = new DynamicParameters();
                parameters.Add("@mgacode", uwStateEntity.MgaCode, DbType.AnsiString, size: 10);
                parameters.Add("@State", uwStateEntity.State, DbType.AnsiString, size: 2);
                parameters.Add("@stbegin", uwStateEntity.STBEGIN, DbType.DateTime);
                parameters.Add("@stexpir", null, DbType.DateTime);
                parameters.Add("@st_tin", uwStateEntity.ST_Tin, DbType.AnsiString, size: 12);
                parameters.Add("@RiskID", uwStateEntity.RiskId, DbType.AnsiString, size: 15);
                parameters.Add("@ResetRating", resetRatingResult, DbType.AnsiStringFixedLength, size: 1);
                parameters.Add("@takeout", null, DbType.AnsiString, size: 10);
                parameters.Add("@RMessage", dbType: DbType.AnsiString, direction: ParameterDirection.Output, size: 1000);

                await _dbConnection.ExecuteAsync(
                    "up_uw_uwstates_insertUpdate",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return parameters.Get<string>("@RMessage");
            }
            catch (SqlException ex)
            {
                var errorMessage = _logger.LogCustomError(
                   new CustomError(_configuration, GlobalErrorCategory.Unhandled, "An unexpected error occurred in GetAllAvailableStatesAsync for dropdown, MgaCode: {MgaCode}", false),
                   LogLevel.Error,
                   ex,
                   nameof(UpsertPolicyStateAsync),
                   null
               );
                throw;
            }
        }
        //add mapping for the above all method in MappingProfile.cs
    }
}

