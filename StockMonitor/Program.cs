using StockMonitor.Components;
using StockMonitor.Hubs;
using StockMonitor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<IMarketDataProvider, FinnhubProvider>();

// Background polling
builder.Services.AddHostedService<MarketPollingService>();

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

// SignalR Hub for Blazor
//app.MapBlazorHub();

// Your custom hub
app.MapHub<MarketHub>("/markethub");

// Blazor (Razor Components)
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
