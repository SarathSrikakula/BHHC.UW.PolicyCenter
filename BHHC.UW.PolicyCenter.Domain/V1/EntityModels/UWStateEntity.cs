using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHHC.UW.PolicyCenter.Domain.V1.EntityModels
{
    public class UWStateEntity
    {
        public string MgaCode { get; set; }
        public string State { get; set; }
        public string StateName { get; set; }
        public DateTime? BeginDate { get; set; }
        public string? StateTin { get; set; }
        public string? RiskId { get; set; }
    }
}
