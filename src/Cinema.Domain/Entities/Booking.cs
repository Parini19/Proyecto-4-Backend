using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace Cinema.Domain.Entities
{
    /// <summary>
    /// Representa una reserva de boletos realizada por un usuario.
    /// Incluye tickets de película y opcionalmente orden de comida.
    /// </summary>
    [FirestoreData]
    public class Booking
    {
        /// <summary>
        /// Identificador único de la reserva (auto-generado)
        /// </summary>
        [FirestoreProperty]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// ID del usuario que realiza la reserva
        /// </summary>
        [FirestoreProperty]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// ID de la función de película (Screening)
        /// </summary>
        [FirestoreProperty]
        public string ScreeningId { get; set; } = string.Empty;

        /// <summary>
        /// Lista de números de asientos reservados (ej: ["A1", "A2", "B5"])
        /// </summary>
        [FirestoreProperty]
        public List<string> SeatNumbers { get; set; } = new List<string>();

        /// <summary>
        /// Cantidad de boletos
        /// </summary>
        [FirestoreProperty]
        public int TicketQuantity { get; set; }

        /// <summary>
        /// Precio por boleto individual
        /// </summary>
        [FirestoreProperty]
        public double TicketPrice { get; set; }

        /// <summary>
        /// Subtotal de los boletos (TicketQuantity * TicketPrice)
        /// </summary>
        [FirestoreProperty]
        public double SubtotalTickets { get; set; }

        /// <summary>
        /// ID de la orden de comida asociada (opcional)
        /// </summary>
        [FirestoreProperty]
        public string? FoodOrderId { get; set; }

        /// <summary>
        /// Subtotal de la comida (0 si no hay orden de comida)
        /// </summary>
        [FirestoreProperty]
        public double SubtotalFood { get; set; }

        /// <summary>
        /// Impuestos aplicados (IVA, etc.)
        /// </summary>
        [FirestoreProperty]
        public double Tax { get; set; }

        /// <summary>
        /// Total a pagar (SubtotalTickets + SubtotalFood + Tax)
        /// </summary>
        [FirestoreProperty]
        public double Total { get; set; }

        /// <summary>
        /// Estado de la reserva: "pending", "confirmed", "cancelled"
        /// </summary>
        [FirestoreProperty]
        public string Status { get; set; } = "pending";

        /// <summary>
        /// Fecha y hora de creación de la reserva
        /// </summary>
        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha y hora de confirmación (cuando se completa el pago)
        /// </summary>
        [FirestoreProperty]
        public DateTime? ConfirmedAt { get; set; }

        /// <summary>
        /// ID del pago asociado a esta reserva
        /// </summary>
        [FirestoreProperty]
        public string? PaymentId { get; set; }
    }
}
