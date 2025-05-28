using Xunit;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;

namespace BHHC.UW.PolicyCenter.Domain.Tests.V1.Models.DTOs
{

    public class ReferenceStateDTOTests
    {
        [Fact]
        public void Properties_AreSetCorrectly()
        {
            var dto = new ReferenceStateDTO { State = "TX", StateName = "Texas" };
            Assert.Equal("TX", dto.State);
            Assert.Equal("Texas", dto.StateName);
        }
    }

}
