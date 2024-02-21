namespace AssafTech.Common.Services;

public class HttpClientService : BaseService, IApiService
{
    private readonly HttpClient _httpClient;
    public HttpClientService(IRepository repository, IMapper mapper, IHttpContextAccessor httpContextAccessor,
        IHttpClientFactory httpClientFactory) : base(repository, mapper, httpContextAccessor)
    {
        _httpClient = httpClientFactory.CreateClient(HttpClientName.AssafTechApiClient);
    }

    public async Task<ResponseModel> GetAsync(EndPointModel endPoint)
    {
        try
        {
            var url = GetUrl(endPoint);
            if (string.IsNullOrWhiteSpace(url))
            {
                ResponseModel.Errors.Add(new ErrorModel((int)CommonResponseCodes.NullReferance, $"{nameof(url)} {ValidationMsg.IsNull}"));
                return ResponseModel;
            }

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<ResponseModel>(responseBody);
            if (responseModel == null)
            {
                ResponseModel.Errors.Add(new ErrorModel((int)CommonResponseCodes.NullReferance, $"{nameof(responseModel)} {ValidationMsg.IsNull}"));
                return ResponseModel;
            }
            return responseModel;
        }
        catch (Exception ex)
        {
            ResponseModel.Errors.Add(new ErrorModel(ex.HResult, ex.Message));
            return ResponseModel;
        }
    }

    public async Task<ResponseModel> PostAsync(EndPointModel endPoint, object payload)
    {
        try
        {
            var url = GetUrl(endPoint);
            if (string.IsNullOrWhiteSpace(url))
            {
                ResponseModel.Errors.Add(new ErrorModel((int)CommonResponseCodes.NullReferance, $"{nameof(url)} {ValidationMsg.IsNull}"));
                return ResponseModel;
            }

            string content = JsonConvert.SerializeObject(payload);
            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(url, httpContent);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<ResponseModel>(responseBody);
            if (responseModel == null)
            {
                ResponseModel.Errors.Add(new ErrorModel((int)CommonResponseCodes.NullReferance, $"{nameof(responseModel)} {ValidationMsg.IsNull}"));
                return ResponseModel;
            }
            return responseModel;
        }
        catch (Exception ex)
        {
            ResponseModel.Errors.Add(new ErrorModel(ex.HResult, ex.Message));
            return ResponseModel;
        }
    }

    public async Task<ResponseModel> PutAsync(EndPointModel endPoint, object payload)
    {
        try
        {
            var url = GetUrl(endPoint);
            if (string.IsNullOrWhiteSpace(url))
            {
                ResponseModel.Errors.Add(new ErrorModel((int)CommonResponseCodes.NullReferance, $"{nameof(url)} {ValidationMsg.IsNull}"));
                return ResponseModel;
            }

            string content = JsonConvert.SerializeObject(payload);
            var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync(url, httpContent);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<ResponseModel>(responseBody);
            if (responseModel == null)
            {
                ResponseModel.Errors.Add(new ErrorModel((int)CommonResponseCodes.NullReferance, $"{nameof(responseModel)} {ValidationMsg.IsNull}"));
                return ResponseModel;
            }
            return responseModel;
        }
        catch (Exception ex)
        {
            ResponseModel.Errors.Add(new ErrorModel(ex.HResult, ex.Message));
            return ResponseModel;
        }
    }

    public async Task<ResponseModel> DeleteAsync(EndPointModel endPoint)
    {
        try
        {
            var url = GetUrl(endPoint);
            if (string.IsNullOrWhiteSpace(url))
            {
                ResponseModel.Errors.Add(new ErrorModel((int)CommonResponseCodes.NullReferance, $"{nameof(url)} {ValidationMsg.IsNull}"));
                return ResponseModel;
            }

            HttpResponseMessage response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var responseModel = JsonConvert.DeserializeObject<ResponseModel>(responseBody);
            if (responseModel == null)
            {
                ResponseModel.Errors.Add(new ErrorModel((int)CommonResponseCodes.NullReferance, $"{nameof(responseModel)} {ValidationMsg.IsNull}"));
                return ResponseModel;
            }
            return responseModel;
        }
        catch (Exception ex)
        {
            ResponseModel.Errors.Add(new ErrorModel(ex.HResult, ex.Message));
            return ResponseModel;
        }
    }

    #region Private
    private string? GetUrl(EndPointModel endPoint)
    {
        if(_httpClient == null || _httpClient.BaseAddress == null) return null;

        var url = $"{_httpClient.BaseAddress}";

        if (!string.IsNullOrWhiteSpace(endPoint.ServiceName))
        {
           url = $"{url}{endPoint.ServiceName}/api/{endPoint.ControllerName}/{endPoint.ActionName}";
        }
        else
        {
           url = $"{url}api/{endPoint.ControllerName}/{endPoint.ActionName}";
        }


        if (!string.IsNullOrWhiteSpace(endPoint.QueryParams))
            url = $"{url}?{endPoint.QueryParams}";

        if (!string.IsNullOrWhiteSpace(endPoint.PathParams))
            url = $"{url}/{endPoint.PathParams}";

        return url;
    }
    #endregion
}


public interface IApiService
{
    Task<ResponseModel> GetAsync(EndPointModel endPoint);
    Task<ResponseModel> PostAsync(EndPointModel endPoint, object payload);
    Task<ResponseModel> PutAsync(EndPointModel endPoint, object payload);
    Task<ResponseModel> DeleteAsync(EndPointModel endPoint);
}
