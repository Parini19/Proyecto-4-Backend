using Google.Cloud.Firestore;
using System;

namespace Cinema.Domain.Entities
{
    /// <summary>
    /// Representa una función/proyección de una película en una sala específica
    /// </summary>
    [FirestoreData]
    public class Screening
    {
        /// <summary>
        /// ID único de la función
        /// </summary>
        [FirestoreProperty]
        public string Id { get; set; }

        /// <summary>
        /// ID de la película que se proyecta
        /// Relación: Screening -> Movie
        /// </summary>
        [FirestoreProperty]
        public string MovieId { get; set; }

        /// <summary>
        /// ID del cine donde se realiza la función
        /// Relación: Screening -> CinemaLocation
        /// Permite filtrar funciones por cine fácilmente
        /// </summary>
        [FirestoreProperty]
        public string CinemaId { get; set; }

        /// <summary>
        /// ID de la sala donde se proyecta
        /// Relación: Screening -> TheaterRoom
        /// </summary>
        [FirestoreProperty]
        public string TheaterRoomId { get; set; }

        /// <summary>
        /// Hora de inicio de la función
        /// </summary>
        [FirestoreProperty]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Hora de finalización de la función
        /// </summary>
        [FirestoreProperty]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Precio de la entrada para esta función
        /// </summary>
        [FirestoreProperty]
        public double Price { get; set; }
    }
}