using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories;
using System.Threading.Tasks;
using BHHC.UW.PolicyCenter.Domain.V1.EntityModels;


namespace BHHC.UW.PolicyCenter.Infrastructure.Tests.V1.Repositories
{
    
public class IStateRepositoryTests
    {
        [Fact]
        public async Task GetPolicyStatesAsync_CanBeCalled()
        {
            var repoMock = new Mock<IStateRepository>();
            repoMock
                .Setup(r => r.GetPolicyStatesAsync("MGA"))
                .ReturnsAsync(new List<UWStateEntity>()); // Fix: Use a compatible type (IEnumerable<UWStateEntity>)

            var result = await repoMock.Object.GetPolicyStatesAsync("MGA");

            Assert.NotNull(result);
        }
    }
}
