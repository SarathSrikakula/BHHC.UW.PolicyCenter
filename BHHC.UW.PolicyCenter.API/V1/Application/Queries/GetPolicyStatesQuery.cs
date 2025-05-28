using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BHHC.UW.PolicyCenter.API.V1.Application.Queries
{
    public class GetPolicyStatesQuery : IRequest<IEnumerable<UWStateDTO>>
    {
        [Required(ErrorMessage = "MGA Code is required to retrieve policy states.")]
        public string MgaCode { get; set; }
    }
}
