using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using MyClass.Core.Data;
using MyClass.Core.Models;
using MyClass.Core.Options;
using MyClass.Core.Services;
using MyClass.Web.App;
using MyClass.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
services.AddRazorComponents()
    .AddInteractiveServerComponents();

services.AddSignalR();
services.AddMudServices();

services.AddDataProtection() // do i need it?
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, ".keys")))
    .SetApplicationName("MyClass");

// database
services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// options
services.Configure<TeacherOptions>(builder.Configuration.GetSection("Teacher"));
services.Configure<QuizOptions>(builder.Configuration.GetSection("Quiz"));

services.AddCoreServices();

// services
services.AddScoped<IQuizNotificationService, SignalRQuizNotificationService>();


var app = builder.Build();

await DatabaseInitializer.InitializeAsync(app.Services);

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
app.MapHub<QuizHub>(QuizHub.Route);
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
