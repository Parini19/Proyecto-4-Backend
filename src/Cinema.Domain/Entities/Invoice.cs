using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace Cinema.Domain.Entities
{
    /// <summary>
    /// Representa una factura generada después de completar una compra.
    /// </summary>
    [FirestoreData]
    public class Invoice
    {
        /// <summary>
        /// Identificador único de la factura
        /// </summary>
        [FirestoreProperty]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// ID de la reserva asociada
        /// </summary>
        [FirestoreProperty]
        public string BookingId { get; set; } = string.Empty;

        /// <summary>
        /// ID del usuario
        /// </summary>
        [FirestoreProperty]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Número de factura secuencial (ej: "INV-2025-0001")
        /// </summary>
        [FirestoreProperty]
        public string InvoiceNumber { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de emisión de la factura
        /// </summary>
        [FirestoreProperty]
        public DateTime IssuedDate { get; set; }

        /// <summary>
        /// Nombre del usuario (cliente)
        /// </summary>
        [FirestoreProperty]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Email del usuario
        /// </summary>
        [FirestoreProperty]
        public string UserEmail { get; set; } = string.Empty;

        /// <summary>
        /// Lista de items en la factura (boletos, comida, etc.)
        /// </summary>
        [FirestoreProperty]
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        /// <summary>
        /// Subtotal antes de impuestos
        /// </summary>
        [FirestoreProperty]
        public double Subtotal { get; set; }

        /// <summary>
        /// Monto de impuestos
        /// </summary>
        [FirestoreProperty]
        public double Tax { get; set; }

        /// <summary>
        /// Total de la factura (Subtotal + Tax)
        /// </summary>
        [FirestoreProperty]
        public double Total { get; set; }

        /// <summary>
        /// Método de pago utilizado
        /// </summary>
        [FirestoreProperty]
        public string PaymentMethod { get; set; } = string.Empty;
    }

    /// <summary>
    /// Representa un item individual en una factura.
    /// </summary>
    [FirestoreData]
    public class InvoiceItem
    {
        /// <summary>
        /// Descripción del item (ej: "Boleto - Avatar", "Combo Grande")
        /// </summary>
        [FirestoreProperty]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad de items
        /// </summary>
        [FirestoreProperty]
        public int Quantity { get; set; }

        /// <summary>
        /// Precio unitario del item
        /// </summary>
        [FirestoreProperty]
        public double UnitPrice { get; set; }

        /// <summary>
        /// Total del item (Quantity * UnitPrice)
        /// </summary>
        [FirestoreProperty]
        public double Total { get; set; }
    }
}
