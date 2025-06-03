using AutoMapper;
using BHHC.UW.PolicyCenter.Domain.V1.EntityModels;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using static BHHC.UW.PolicyCenter.API.PolicyCenterException.HandledException;

namespace BHHC.UW.PolicyCenter.API.V1.Application.Queries
{
    public class GetPolicyStatesQueryTests : IRequest<IEnumerable<PolicyAssignedStatesDTO>>
    {
        public class GetPolicyStatesHandlerTests
        {
            private readonly Mock<IStateRepository> _stateRepositoryMock;
            private readonly Mock<ILogger<GetPolicyStatesHandler>> _loggerMock;
            private readonly Mock<IMapper> _mapperMock;
            private readonly GetPolicyStatesHandler _handler;
            private readonly Mock<IConfiguration> _configurationMock;

            public GetPolicyStatesHandlerTests()
            {
                _stateRepositoryMock = new Mock<IStateRepository>();
                _loggerMock = new Mock<ILogger<GetPolicyStatesHandler>>();
                _mapperMock = new Mock<IMapper>();
                _configurationMock = new Mock<IConfiguration>();
                _handler = new GetPolicyStatesHandler(_stateRepositoryMock.Object, _loggerMock.Object, _mapperMock.Object, _configurationMock.Object);
            }

            [Fact]
            public async Task Handle_ValidMgaCode_ReturnsMappedStates()
            {
                // Arrange
                var mgaCode = "TX";
                var query = new GetPolicyStatesQuery { MgaCode = mgaCode };
                var entityStates = new List<UWStateEntity>
                    {
                        new UWStateEntity { MgaCode = "TX", State = "Texas", StateName = "Texas" },
                        new UWStateEntity { MgaCode = "TX", State = "California", StateName = "California" }
                    };

                var dtoStates = new List<PolicyAssignedStatesDTO>
                    {
                        new PolicyAssignedStatesDTO
                        {
                            PolicyCode = "TX",
                            State = "Texas",
                            StateName = "Texas",
                            StateEffectiveDate = new DateTime(2020, 1, 1),
                            StateEmployerCode = "TIN123",
                            BureauId = "RISK001"
                        },
                        new PolicyAssignedStatesDTO
                        {
                            PolicyCode = "TX",
                            State = "California",
                            StateName = "California",
                            StateEffectiveDate = new DateTime(2021, 2, 2),
                            StateEmployerCode = "TIN456",
                            BureauId = "RISK002"
                        }
                    };

                var dtoStates1 = new List<PolicyAssignedStatesDTO1>
                    {
                        new PolicyAssignedStatesDTO1
                        {
                            PolicyCode = "TX",
                            State = "Texas",
                            StateName = "Texas",
                            StateEffectiveDate = new DateTime(2020, 1, 1),
                            StateEmployerCode = "TIN123",
                            BureauId = "RISK001"
                        },
                        new PolicyAssignedStatesDTO1
                        {
                            PolicyCode = "TX",
                            State = "California",
                            StateName = "California",
                            StateEffectiveDate = new DateTime(2021, 2, 2),
                            StateEmployerCode = "TIN456",
                            BureauId = "RISK002"
                        }
                    };

                _stateRepositoryMock.Setup(r => r.GetPolicyStatesAsync(mgaCode))
                    .ReturnsAsync(dtoStates);
                _mapperMock.Setup(m => m.Map<IEnumerable<PolicyAssignedStatesDTO1>>(entityStates))
                    .Returns(dtoStates1);

                // Act
                var result = await _handler.Handle(query, CancellationToken.None);

                // Assert
                Assert.NotNull(result);
            }
        }
    }

}
