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
        public string Stateabb { get; set; }
        public DateTime? STBEGIN { get; set; }
        public string? ST_Tin { get; set; }
        public string? RiskId { get; set; }
    }
}
