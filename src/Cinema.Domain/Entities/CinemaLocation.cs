using Google.Cloud.Firestore;
using System;

namespace Cinema.Domain.Entities
{
    /// <summary>
    /// Representa un cine/sede física con múltiples salas
    /// Ejemplo: "Cine Premium San José", "Cine Mall Escazú"
    /// </summary>
    [FirestoreData]
    public class CinemaLocation
    {
        /// <summary>
        /// ID único del cine
        /// </summary>
        [FirestoreProperty]
        public string Id { get; set; }

        /// <summary>
        /// Nombre del cine/sede
        /// Ejemplo: "Cine Premium San José"
        /// </summary>
        [FirestoreProperty]
        public string Name { get; set; }

        /// <summary>
        /// Ciudad donde se ubica el cine
        /// Ejemplo: "San José", "Heredia", "Escazú"
        /// </summary>
        [FirestoreProperty]
        public string City { get; set; }

        /// <summary>
        /// Dirección completa del cine
        /// </summary>
        [FirestoreProperty]
        public string Address { get; set; }

        /// <summary>
        /// Número de teléfono del cine
        /// </summary>
        [FirestoreProperty]
        public string Phone { get; set; }

        /// <summary>
        /// URL de imagen/logo del cine (opcional)
        /// </summary>
        [FirestoreProperty]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Indica si el cine está activo y aceptando funciones
        /// </summary>
        [FirestoreProperty]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Fecha de creación del cine
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
