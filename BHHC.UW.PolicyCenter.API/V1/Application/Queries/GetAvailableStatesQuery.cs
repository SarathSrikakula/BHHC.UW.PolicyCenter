using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BHHC.UW.PolicyCenter.API.V1.Application.Queries
{
    public class GetAvailableStatesQuery : IRequest<IEnumerable<PolicyAvailableStatesDTO1>>
    {
        [Required(ErrorMessage = "MGA Code is required.")]
        [StringLength(10, ErrorMessage = "MGA Code cannot be longer than 10 characters.")]
        public string MgaCode { get; set; }
    }
}
