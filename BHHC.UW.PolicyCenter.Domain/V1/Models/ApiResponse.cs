using Newtonsoft.Json;

namespace BHHC.UW.PolicyCenter.API.V1.Application.Models
{
    public class ApiResponse<T> : IApiResponse<T>
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("result")]
        public T Result { get; set; }

        [JsonProperty("exceptions")]
        public ApiException Exception { get; set; }
        public string ValidationMessage { get; set; }
    }
}
