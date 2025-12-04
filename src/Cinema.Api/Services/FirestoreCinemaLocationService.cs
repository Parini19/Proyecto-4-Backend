using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinema.Domain.Entities;

namespace Cinema.Api.Services
{
    /// <summary>
    /// Servicio para manejar operaciones CRUD de cines/sedes en Firestore
    /// </summary>
    public class FirestoreCinemaLocationService
    {
        private readonly FirestoreDb _firestoreDb;
        private const string CollectionName = "cinemaLocations";

        public FirestoreCinemaLocationService(IConfiguration configuration)
        {
            var projectId = configuration["Firebase:ProjectId"];
            var configPath = configuration["Firebase:ConfigPath"];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", configPath);
            _firestoreDb = FirestoreDb.Create(projectId);
        }

        /// <summary>
        /// Crea un nuevo cine en Firestore
        /// </summary>
        public async Task<CinemaLocation> AddCinemaLocationAsync(CinemaLocation cinema)
        {
            if (string.IsNullOrEmpty(cinema.Id))
            {
                cinema.Id = Guid.NewGuid().ToString();
            }

            cinema.CreatedAt = DateTime.UtcNow;
            cinema.UpdatedAt = DateTime.UtcNow;

            var docRef = _firestoreDb.Collection(CollectionName).Document(cinema.Id);
            await docRef.SetAsync(cinema);

            return cinema;
        }

        /// <summary>
        /// Obtiene un cine por su ID
        /// </summary>
        public async Task<CinemaLocation?> GetCinemaLocationAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists)
                return null;

            return snapshot.ConvertTo<CinemaLocation>();
        }

        /// <summary>
        /// Obtiene todos los cines
        /// </summary>
        public async Task<List<CinemaLocation>> GetAllCinemaLocationsAsync()
        {
            var snapshot = await _firestoreDb.Collection(CollectionName).GetSnapshotAsync();
            var cinemas = new List<CinemaLocation>();

            foreach (var doc in snapshot.Documents)
            {
                cinemas.Add(doc.ConvertTo<CinemaLocation>());
            }

            return cinemas.OrderBy(c => c.Name).ToList();
        }

        /// <summary>
        /// Obtiene solo los cines activos
        /// </summary>
        public async Task<List<CinemaLocation>> GetActiveCinemaLocationsAsync()
        {
            var query = _firestoreDb.Collection(CollectionName)
                .WhereEqualTo("IsActive", true);

            var snapshot = await query.GetSnapshotAsync();
            var cinemas = new List<CinemaLocation>();

            foreach (var doc in snapshot.Documents)
            {
                cinemas.Add(doc.ConvertTo<CinemaLocation>());
            }

            return cinemas.OrderBy(c => c.Name).ToList();
        }

        /// <summary>
        /// Obtiene cines por ciudad
        /// </summary>
        public async Task<List<CinemaLocation>> GetCinemaLocationsByCityAsync(string city)
        {
            var query = _firestoreDb.Collection(CollectionName)
                .WhereEqualTo("City", city)
                .WhereEqualTo("IsActive", true);

            var snapshot = await query.GetSnapshotAsync();
            var cinemas = new List<CinemaLocation>();

            foreach (var doc in snapshot.Documents)
            {
                cinemas.Add(doc.ConvertTo<CinemaLocation>());
            }

            return cinemas.OrderBy(c => c.Name).ToList();
        }

        /// <summary>
        /// Actualiza un cine existente
        /// </summary>
        public async Task UpdateCinemaLocationAsync(CinemaLocation cinema)
        {
            cinema.UpdatedAt = DateTime.UtcNow;

            var docRef = _firestoreDb.Collection(CollectionName).Document(cinema.Id);
            await docRef.SetAsync(cinema, SetOptions.Overwrite);
        }

        /// <summary>
        /// Elimina un cine
        /// IMPORTANTE: Antes de eliminar, verificar que no tenga salas o funciones asociadas
        /// </summary>
        public async Task DeleteCinemaLocationAsync(string id)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            await docRef.DeleteAsync();
        }

        /// <summary>
        /// Activa o desactiva un cine
        /// </summary>
        public async Task ToggleCinemaLocationStatusAsync(string id, bool isActive)
        {
            var docRef = _firestoreDb.Collection(CollectionName).Document(id);
            await docRef.UpdateAsync(new Dictionary<string, object>
            {
                { "IsActive", isActive },
                { "UpdatedAt", DateTime.UtcNow }
            });
        }

        /// <summary>
        /// Obtiene estadísticas de un cine (salas, funciones, etc.)
        /// </summary>
        public async Task<CinemaStats> GetCinemaStatsAsync(string cinemaId)
        {
            // Contar salas
            var roomsSnapshot = await _firestoreDb.Collection("theaterRooms")
                .WhereEqualTo("CinemaId", cinemaId)
                .GetSnapshotAsync();
            var roomsCount = roomsSnapshot.Count;

            // Contar funciones (hoy)
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var screeningsSnapshot = await _firestoreDb.Collection("screenings")
                .WhereEqualTo("CinemaId", cinemaId)
                .WhereGreaterThanOrEqualTo("StartTime", today)
                .WhereLessThan("StartTime", tomorrow)
                .GetSnapshotAsync();
            var screeningsToday = screeningsSnapshot.Count;

            return new CinemaStats
            {
                CinemaId = cinemaId,
                TotalRooms = roomsCount,
                ScreeningsToday = screeningsToday
            };
        }
    }

    /// <summary>
    /// Estadísticas de un cine
    /// </summary>
    public class CinemaStats
    {
        public string CinemaId { get; set; }
        public int TotalRooms { get; set; }
        public int ScreeningsToday { get; set; }
    }
}
