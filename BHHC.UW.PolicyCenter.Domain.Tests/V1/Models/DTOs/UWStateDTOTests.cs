using Xunit;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;

namespace BHHC.UW.PolicyCenter.Domain.Tests.V1.Models.DTOs
{
    

    public class UWStateDTOTests
    {
        [Fact]
        public void Properties_AreSetCorrectly()
        {
            var dto = new UWStateDTO { State = "CA", StateName = "California" };
            Assert.Equal("CA", dto.State);
            Assert.Equal("California", dto.StateName);
        }
    }
}
