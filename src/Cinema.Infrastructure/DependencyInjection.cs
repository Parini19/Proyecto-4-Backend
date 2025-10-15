using Cinema.Application.Movies;
using Cinema.Infrastructure.Repositories;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Cinema.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var enabled = config.GetValue<bool?>("Firebase:Enabled") ?? false;

        if (!enabled)
        {
            Log.Warning("Firebase deshabilitado (Firebase:Enabled=false). Usando InMemoryMovieRepository.");
            services.AddScoped<IMovieRepository, InMemoryMovieRepository>();
            return services;
        }

        var projectId = config["Firebase:ProjectId"];
        if (string.IsNullOrWhiteSpace(projectId))
        {
            Log.Warning("Firebase:ProjectId vacío. Usando InMemoryMovieRepository.");
            services.AddScoped<IMovieRepository, InMemoryMovieRepository>();
            return services;
        }

        var serviceAccountPath = config["Firebase:ServiceAccountPath"];

        try
        {
            if (!string.IsNullOrWhiteSpace(serviceAccountPath) && File.Exists(serviceAccountPath))
            {
                if (FirebaseApp.DefaultInstance is null)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(serviceAccountPath)
                    });
                    Log.Information("FirebaseApp inicializado con service account.");
                }
            }
            else
            {
                Log.Warning("Service account no encontrado en {Path}. Intentando credenciales por defecto.", serviceAccountPath);
                if (FirebaseApp.DefaultInstance is null)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.GetApplicationDefault()
                    });
                }
            }

            services.AddSingleton(sp => new FirestoreDbBuilder { ProjectId = projectId }.Build());
            services.AddScoped<IMovieRepository, FirestoreUserRepository>();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error inicializando Firebase/Firestore. Fallback a InMemoryMovieRepository.");
            services.AddScoped<IMovieRepository, InMemoryMovieRepository>();
        }

        return services;
    }
}