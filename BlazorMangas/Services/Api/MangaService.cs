using BlazorMangas.Models.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BlazorMangas.Services.Api;

public class MangaService : IMangaService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public ILogger<CategoriaService> _logger;
    private const string apiEndpoint = "/api/mangas/";
    private readonly JsonSerializerOptions _options;

    private MangaDTO? manga;
    private List<MangaDTO>? mangas;

    public MangaService(IHttpClientFactory httpClientFactory,
        ILogger<CategoriaService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }
    public async Task<IEnumerable<MangaDTO>> GetMangas()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("ApiMangas");
            var result = await httpClient.GetFromJsonAsync<List<MangaDTO>>(apiEndpoint);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao acessar categorias: {apiEndpoint} " + ex.Message);
            throw new UnauthorizedAccessException();
        }
    }

    public async Task<MangaDTO> CreateManga(MangaDTO mangaDto)
    {
        var httpClient = _httpClientFactory.CreateClient("ApiMangas");

        StringContent content = new StringContent(JsonSerializer.Serialize(mangaDto),
                                                  Encoding.UTF8, "application/json");

        using (var response = await httpClient.PostAsync(apiEndpoint, content))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();

                manga = await JsonSerializer
                           .DeserializeAsync<MangaDTO>(apiResponse, _options);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }
            else
            {
                return null;
            }
        }
        return manga;
    }

    public async Task<bool> DeleteManga(int id)
    {
        var httpClient = _httpClientFactory.CreateClient("ApiMangas");

        using (var response = await httpClient.DeleteAsync(apiEndpoint + id))
        {
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }
        }
        return false;
    }

    public async Task<MangaDTO> GetManga(int id)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("ApiMangas");
            var response = await httpClient.GetAsync(apiEndpoint + id);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<MangaDTO>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Erro ao obter o mangá pelo id= {id} - {message}");
                throw new Exception($"Status Code : {response.StatusCode} - {message}");
            }
        }
        catch (UnauthorizedAccessException)
        {
            throw new UnauthorizedAccessException();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter o mangá pelo id={id} \n\n {ex.Message}");
            throw;
        }
    }

    public Task<IEnumerable<MangaDTO>> GetMangaPorCategoria(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<MangaDTO> UpdateManga(int id, MangaDTO mangaDto)
    {
        var httpClient = _httpClientFactory.CreateClient("ApiMangas");

        MangaDTO mangaUpdated = new MangaDTO();

        using (var response = await httpClient.PutAsJsonAsync(apiEndpoint + id, mangaDto))
        {
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStreamAsync();
                mangaUpdated = await JsonSerializer
                                    .DeserializeAsync<MangaDTO>(apiResponse, _options);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }
        }
        return mangaUpdated;
    }
}
