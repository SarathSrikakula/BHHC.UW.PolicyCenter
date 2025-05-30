namespace BHHC.UW.PolicyCenter.API.V1.Application.Commands
{
   
    public class UpsertPolicyStateCommandRequest
    {
        public string MgaCode { get; set; }
        public DateTime? StateBeginDate { get; set; }
        public string? StateTin { get; set; }
        public string? RiskId { get; set; }
        public string State { get; set; }
    }
}
