using VcHubSignalRDemo.Components;
using VcHubSignalRDemo.Auth;
using VcHubSignalRDemo.Configuration;
using VcHubSignalRDemo.SignalR;
using VcHubSignalRDemo.State;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMemoryCache();
builder.Services.Configure<OpenApiOptions>(builder.Configuration.GetSection("OpenApi"));
builder.Services.Configure<SignalROptions>(builder.Configuration.GetSection("SignalR"));
builder.Services.Configure<DemoOptions>(builder.Configuration.GetSection("Demo"));

builder.Services.AddScoped<ConnectionStateStore>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IVcHubConnectionFactory, VcHubConnectionFactory>();
builder.Services.AddScoped<ISignalRStreamService, SignalRStreamService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
