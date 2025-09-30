using Data.Contexts;
using Data.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== EF Core =====
builder.Services.AddDbContext<DataContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// ===== DataProtection (delad nyckelring mellan tjänster) =====
var keysPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "CoreGym", "dp-keys"
);
Directory.CreateDirectory(keysPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("CoreGym"); // MÅSTE matcha AuthSystem
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("CoreGym"); // MÅSTE matcha AuthSystem

builder.Services.AddScoped<IAuthService, AuthService>();

// ===== Identity (utfärdar cookie) =====
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
{
    opt.Password.RequiredLength = 6;
    opt.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

// ===== Cookie Auth =====
builder.Services.ConfigureApplicationCookie(o =>
{
    o.Cookie.Name = ".myapp.id";
    o.Cookie.HttpOnly = true;
    o.Cookie.SameSite = SameSiteMode.None;             // för cross-site
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always; // kräver HTTPS

    // API ska inte redirecta, returnera koder:
    o.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
    {
        OnRedirectToLogin = ctx => { ctx.Response.StatusCode = 401; return Task.CompletedTask; },
        OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = 403; return Task.CompletedTask; }
    };
});

builder.Services.AddAuthorization();

// ===== CORS: hårdkodat origin =====
const string SpaCors = "spa";
string[] allowedOrigins = { "https://fantasticg5-dmdbeshvcmfxe6ey.northeurope-01.azurewebsites.net", "http://localhost:5173", "https://localhost:5173" };

builder.Services.AddCors(opt =>
{
    opt.AddPolicy(SpaCors, p => p
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API"));

app.UseHttpsRedirection();
app.UseCors(SpaCors);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
