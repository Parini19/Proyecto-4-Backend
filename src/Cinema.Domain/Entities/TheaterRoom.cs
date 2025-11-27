using Google.Cloud.Firestore;
using System;

namespace Cinema.Domain.Entities
{
    /// <summary>
    /// Representa una sala de cine dentro de un cine/sede
    /// Ejemplo: "Sala 1", "Sala VIP", "Sala IMAX"
    /// </summary>
    [FirestoreData]
    public class TheaterRoom
    {
        /// <summary>
        /// ID único de la sala
        /// </summary>
        [FirestoreProperty]
        public string Id { get; set; }

        /// <summary>
        /// ID del cine al que pertenece esta sala
        /// Relación: TheaterRoom -> CinemaLocation
        /// </summary>
        [FirestoreProperty]
        public string CinemaId { get; set; }

        /// <summary>
        /// Nombre de la sala
        /// Ejemplo: "Sala 1", "Sala VIP", "Sala IMAX"
        /// </summary>
        [FirestoreProperty]
        public string Name { get; set; }

        /// <summary>
        /// Capacidad total de asientos de la sala
        /// Se calcula automáticamente basado en la configuración de asientos
        /// </summary>
        [FirestoreProperty]
        public int Capacity { get; set; }

        /// <summary>
        /// Configuración de asientos en formato JSON
        /// Contiene la disposición visual de los asientos (filas, columnas, pasillos, tipos)
        /// </summary>
        [FirestoreProperty]
        public string SeatConfiguration { get; set; }

        /// <summary>
        /// Fecha de creación de la sala
        /// </summary>
        [FirestoreProperty]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Última actualización
        /// </summary>
        [FirestoreProperty]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}