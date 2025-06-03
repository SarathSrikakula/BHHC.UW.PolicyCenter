using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BHHC.UW.PolicyCenter.API.V1.Application.Commands
{
    public class UpsertPolicyState : IRequest<string>
    {
        [Required(ErrorMessage = "MGA Code is required.")]
        [StringLength(10, ErrorMessage = "MGA Code cannot be longer than 10 characters.")]
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
        [StringLength(10, ErrorMessage = "MGA Code cannot be longer than 10 characters.")]
        public string MgaCode { get; set; }
        public DateTime? StateBeginDate { get; set; }
        public string? StateTin { get; set; }
        public string? RiskId { get; set; }
        [Required(ErrorMessage = "State is required.")]
        public string State { get; set; }
    }
    //for all the MgaCode please use string lenght to 10 validation in command and query object excepty entity
}
