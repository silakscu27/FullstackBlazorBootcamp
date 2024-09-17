using BlazorOpenAIStarter;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var openAIApiKey = builder
    .Configuration
    .GetSection("OpenAIApiKey")
    .Value;

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOpenAIService(settings => settings.ApiKey = openAIApiKey);

await builder.Build().RunAsync();
