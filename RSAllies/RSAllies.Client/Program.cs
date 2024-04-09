using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using RSAllies.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);



builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

builder.Services.AddFluentUIComponents();
builder.Services.AddScoped<DialogService>();

await builder.Build().RunAsync();
