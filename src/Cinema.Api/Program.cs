using Cinema.Api.Services;
using Cinema.Application.Cinema;
using Cinema.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Cinema.Api.Services;
using Microsoft.FeatureManagement;

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

// Activar Auth/JWT m�s adelante
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
<<<<<<< Updated upstream
builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();
=======
builder.Services.AddScoped<FirestoreUserService>();
builder.Services.AddScoped<FirestoreMovieService>();
builder.Services.AddScoped<FirestoreScreeningService>();
builder.Services.AddScoped<FirestoreFoodComboService>();
builder.Services.AddScoped<FirestoreTheaterRoomService>();
builder.Services.AddScoped<FirestoreFoodOrderService>();
builder.Services.AddFeatureManagement();
>>>>>>> Stashed changes

//FireStore
builder.Services.AddScoped<FirestoreUserService>();
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

// Solo usar HTTPS redirect en producción para evitar problemas con CORS en desarrollo
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("FlutterClient");

if (firebaseEnabled)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

// Middleware de auditor�a
app.UseMiddleware<Cinema.Api.Utilities.UserActionAuditMiddleware>();

app.MapControllers();

// Health p�blico
app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTime.UtcNow }))
   .AllowAnonymous();

app.Run();
