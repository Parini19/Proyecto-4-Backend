using Cinema.Api.Services;
using Cinema.Application.Cinema;
using Cinema.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext());

// CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FlutterClient", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Firebase (apagado en esta fase)
// Usa un flag en appsettings: "Firebase": { "Enabled": false }
var firebaseEnabled = builder.Configuration.GetValue<bool>("Firebase:Enabled");

// Activar Auth/JWT más adelante
if (firebaseEnabled)
{
    var projectId = builder.Configuration["Firebase:ProjectId"]
        ?? throw new InvalidOperationException("Firebase:ProjectId missing");

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = $"https://securetoken.google.com/{projectId}";
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = $"https://securetoken.google.com/{projectId}",
                ValidateAudience = true,
                ValidAudience = projectId,
                ValidateLifetime = true
            };
        });

    builder.Services.AddAuthorization();
}
//FeatureFlags
builder.Services.AddFeatureManagement();

// Infraestructura (elige repo en memoria si Firebase:Enabled=false)
Cinema.Infrastructure.DependencyInjection.AddInfrastructure(builder.Services, builder.Configuration);
builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();

//FireStore
builder.Services.AddSingleton<FirestoreUserService>();
var app = builder.Build();

app.UseSerilogRequestLogging();

// Swagger en Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // "/" lleva swagger en dev
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();

app.UseCors("FlutterClient");

if (firebaseEnabled)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

// Middleware de auditoría
app.UseMiddleware<Cinema.Api.Utilities.UserActionAuditMiddleware>();

app.MapControllers();

// Health público
app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTime.UtcNow }))
   .AllowAnonymous();

app.Run();
