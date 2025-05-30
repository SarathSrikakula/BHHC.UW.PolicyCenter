using BHHC.UW.PolicyCenter.API.V1.Application.Commands;
using BHHC.UW.PolicyCenter.API.V1.Application.Models;
using BHHC.UW.PolicyCenter.Domain.V1.Models.DTOs;

namespace app_uwpolicicenter.ServiceClients
{
    public interface IPolicyStatesServiceClient
    {
        Task<ApiResponse<IEnumerable<UWStateDTO>>> GetPolicyStatesAsync(string policyId);
        Task<ApiResponse<string>> UpsertPolicyStateAsync(UpsertPolicyStateCommandRequest request);
        Task<IEnumerable<ReferenceStateDTO>> GetAvailableStatesAsync(string policyId);
    }
}
