using Google.Cloud.Firestore;
using System;

namespace Cinema.Domain.Entities
{
    /// <summary>
    /// Representa un pago simulado para una reserva.
    /// NOTA: Este es un sistema de pagos simulado para propósitos educativos.
    /// No se procesan pagos reales.
    /// </summary>
    [FirestoreData]
    public class Payment
    {
        /// <summary>
        /// Identificador único del pago
        /// </summary>
        [FirestoreProperty]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// ID de la reserva asociada
        /// </summary>
        [FirestoreProperty]
        public string BookingId { get; set; } = string.Empty;

        /// <summary>
        /// ID del usuario que realiza el pago
        /// </summary>
        [FirestoreProperty]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Monto total del pago
        /// </summary>
        [FirestoreProperty]
        public double Amount { get; set; }

        /// <summary>
        /// Método de pago: "credit_card", "debit_card", "cash"
        /// </summary>
        [FirestoreProperty]
        public string PaymentMethod { get; set; } = string.Empty;

        /// <summary>
        /// Últimos 4 dígitos de la tarjeta (para simulación y visualización)
        /// </summary>
        [FirestoreProperty]
        public string CardLastFourDigits { get; set; } = string.Empty;

        /// <summary>
        /// Marca de la tarjeta: "Visa", "MasterCard", "American Express"
        /// </summary>
        [FirestoreProperty]
        public string CardBrand { get; set; } = string.Empty;

        /// <summary>
        /// ID de transacción generado (simulado)
        /// </summary>
        [FirestoreProperty]
        public string TransactionId { get; set; } = string.Empty;

        /// <summary>
        /// Estado del pago: "pending", "approved", "rejected"
        /// </summary>
        [FirestoreProperty]
        public string Status { get; set; } = "pending";

        /// <summary>
        /// Fecha y hora de creación del pago
        /// </summary>
        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha y hora de procesamiento del pago
        /// </summary>
        [FirestoreProperty]
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Razón de rechazo (si el pago fue rechazado)
        /// </summary>
        [FirestoreProperty]
        public string? RejectionReason { get; set; }
    }
}
