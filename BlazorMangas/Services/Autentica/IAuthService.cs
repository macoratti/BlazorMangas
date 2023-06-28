using BlazorMangas.Models;

namespace BlazorMangas.Services.Autentica;

public interface IAuthService
{
    Task<LoginResult> Login(LoginModel loginModel);

    Task Logout();
}
