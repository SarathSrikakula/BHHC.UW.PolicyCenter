using BHHC.UW.PolicyCenter.Domain.V1.EntityModels;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHHC.UW.PolicyCenter.Infrastructure.V1.Repositories
{
    public interface IStateRepository
    {
        Task<IEnumerable<UWStateEntity>> GetPolicyStatesAsync(string mgaCode);
        Task<IEnumerable<ReferenceStateDTO>> GetAllAvailableStatesAsync(string mgacode); // Added mgacode parameter
        Task<string> UpsertPolicyStateAsync(UWStateEntity uwStateEntity);
    }
}
