namespace BlazorMangas.Models;

public class LoginResult
{
    public string? Error { get; set; }
    public string? Token { get; set; }
    public string? Expiration { get; set; }
}
