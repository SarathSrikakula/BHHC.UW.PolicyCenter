namespace BHHC.UW.PolicyCenter.API.V1.Application.Models
{
    public interface IApiResponse<T>
    {
        bool Success { get; set; }
        T Result { get; set; }
        ApiException Exception { get; set; }
    }

}
