using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs
{
    // Models/UWStateDTO.cs
    // This is the DTO (Data Transfer Object) for API responses for policy states.
    // Renamed to UWStateDTO for clarity.
    public class PolicyAssignedStatesDTO
    {
        public string PolicyCode { get; set; }
        public string State { get; set; }
        public DateTime StateEffectiveDate { get; set; }
        public string StateEmployerCode { get; set; }
        public string BureauId { get; set; }
        public string StateName { get; set; }
    }
}
