using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BHHC.UW.PolicyCenter.API.V1.Application.Commands
{
    public class UpsertPolicyStateCommand : IRequest<string>
    {
        [Required(ErrorMessage = "MGA Code is required.")]
        public string MgaCode { get; set; }
        public DateTime? StateBeginDate { get; set; }
        public string? StateTin { get; set; }
        public string? RiskId { get; set; }
        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }
    }

    public class UpsertPolicyStateCommandRequest
    {
        [Required(ErrorMessage = "MGA Code is required.")]
        public string MgaCode { get; set; }
        public DateTime? StateBeginDate { get; set; }
        public string? StateTin { get; set; }
        public string? RiskId { get; set; }
        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }
    }
}
