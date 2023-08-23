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

    private int QuantidadeTotalPaginas;
    private MangaPaginacaoResponseDTO? responsePaginacaoDTO;

    public MangaService(IHttpClientFactory httpClientFactory,
        ILogger<CategoriaService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<IEnumerable<MangaDTO>> GetMangasPorTitulo(string titulo)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("ApiMangas");
            var response = await httpClient.GetAsync(apiEndpoint + "search/" + titulo);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<MangaDTO>>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Erro ao obter mangás com titulo {titulo} - {message}");
                throw new Exception($"Status Code : {response.StatusCode} - {message}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter o mangá pelo titulo={titulo} \n\n {ex.Message}");
            throw;
        }
    }
    public async Task<MangaPaginacaoResponseDTO> GetMangasPaginacao(int pagina,
        int quantidadePorPagina)
    {
        var caminho = $"paginacao?pagina={pagina}&quantidadePorPagina={quantidadePorPagina}";
        var apiUrl = apiEndpoint + caminho;
        try
        {
            var httpClient = _httpClientFactory.CreateClient("ApiMangas");
            var httpResponse = await httpClient.GetAsync(apiUrl);

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseString = await httpResponse.Content.ReadAsStringAsync();
                responsePaginacaoDTO =
                    JsonSerializer.Deserialize<MangaPaginacaoResponseDTO>
                    (responseString, _options);

                QuantidadeTotalPaginas = responsePaginacaoDTO.TotalPaginas;
                mangas = responsePaginacaoDTO.Mangas;
            }
            else
            {
                _logger.LogWarning("O request para a API falhou com o status: "
                    + httpResponse.StatusCode);
            }
            return responsePaginacaoDTO;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao acessar categorias: " +
                $"{apiEndpoint}/paginacao" + ex.Message);
            throw new UnauthorizedAccessException();
        }
    }

    public async Task<IEnumerable<MangaDTO>> GetMangas()
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("ApiMangas");
            var result = await httpClient.GetFromJsonAsync<IEnumerable<MangaDTO>>(apiEndpoint);
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

    public async Task<IEnumerable<MangaDTO>> GetMangasPorCategoria(int id)
    {
        try
        {
            var httpClient = _httpClientFactory.CreateClient("ApiMangas");
            var response = await httpClient.GetAsync(apiEndpoint + "get-mangas-by-category/" + id);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<IEnumerable<MangaDTO>>();
            }
            else
            {
                var message = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Erro ao obter os mangás pelo id= {id} - {message}");
                throw new Exception($"Status Code : {response.StatusCode} - {message}");
            }
        }
        catch (UnauthorizedAccessException)
        {
            throw new UnauthorizedAccessException();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter os mangás pelo id={id} \n\n {ex.Message}");
            throw;
        }
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
