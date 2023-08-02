namespace BlazorMangas.Services.Api;

public interface IFileUploadService
{
    Task<HttpResponseMessage> UploadFileAsync(
             string endpoint, MultipartFormDataContent content);
}
