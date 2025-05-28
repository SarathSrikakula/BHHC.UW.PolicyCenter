using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using MediatR;

namespace BHHC.UW.PolicyCenter.API.V1.Application.Queries
{
    public class GetAvailableStatesQuery : IRequest<IEnumerable<ReferenceStateDTO>>
    {
        public string MgaCode { get; set; }
    }
}
