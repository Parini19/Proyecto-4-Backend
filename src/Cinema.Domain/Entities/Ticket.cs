using Google.Cloud.Firestore;
using System;

namespace Cinema.Domain.Entities
{
    /// <summary>
    /// Representa un boleto digital individual con código QR.
    /// Cada boleto es para un asiento específico en una función.
    /// </summary>
    [FirestoreData]
    public class Ticket
    {
        /// <summary>
        /// Identificador único del boleto
        /// </summary>
        [FirestoreProperty]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// ID de la reserva a la que pertenece este boleto
        /// </summary>
        [FirestoreProperty]
        public string BookingId { get; set; } = string.Empty;

        /// <summary>
        /// ID del usuario propietario del boleto
        /// </summary>
        [FirestoreProperty]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// ID de la función de película
        /// </summary>
        [FirestoreProperty]
        public string ScreeningId { get; set; } = string.Empty;

        /// <summary>
        /// Título de la película (desnormalizado para facilitar consultas)
        /// </summary>
        [FirestoreProperty]
        public string MovieTitle { get; set; } = string.Empty;

        /// <summary>
        /// Nombre de la sala de cine
        /// </summary>
        [FirestoreProperty]
        public string TheaterRoomName { get; set; } = string.Empty;

        /// <summary>
        /// Número de asiento asignado (ej: "A1", "B5")
        /// </summary>
        [FirestoreProperty]
        public string SeatNumber { get; set; } = string.Empty;

        /// <summary>
        /// Fecha y hora de la función
        /// </summary>
        [FirestoreProperty]
        public DateTime ShowTime { get; set; }

        /// <summary>
        /// Código QR en formato Base64 (imagen)
        /// </summary>
        [FirestoreProperty]
        public string QrCode { get; set; } = string.Empty;

        /// <summary>
        /// Datos codificados en el QR (string original antes de generar la imagen)
        /// Formato: TICKET:id=XXX|user=YYY|screening=ZZZ|seat=AAA|showtime=...
        /// </summary>
        [FirestoreProperty]
        public string QrCodeData { get; set; } = string.Empty;

        /// <summary>
        /// Indica si el boleto ya fue usado (escaneado en la entrada)
        /// </summary>
        [FirestoreProperty]
        public bool IsUsed { get; set; } = false;

        /// <summary>
        /// Fecha y hora en que se usó el boleto (escaneado)
        /// </summary>
        [FirestoreProperty]
        public DateTime? UsedAt { get; set; }

        /// <summary>
        /// Fecha y hora de creación del boleto
        /// </summary>
        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de expiración del boleto (ShowTime + 30 minutos)
        /// </summary>
        [FirestoreProperty]
        public DateTime ExpiresAt { get; set; }
    }
}
