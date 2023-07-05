using Blazored.LocalStorage;
using BlazorMangas;
using BlazorMangas.Services.Api;
using BlazorMangas.Services.Autentica;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("ApiMangas", options =>
{
    options.BaseAddress = new Uri("https://localhost:7020/");
}).AddHttpMessageHandler<CustomHttpHandler>();

builder.Services.AddScoped<CustomHttpHandler>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();

//servi�o de categorias
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

await builder.Build().RunAsync();
