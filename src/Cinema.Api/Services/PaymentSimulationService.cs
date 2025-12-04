using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Api.Services
{
    /// <summary>
    /// Servicio para simular procesamiento de pagos.
    /// IMPORTANTE: Este servicio NO procesa pagos reales. Es solo para propósitos educativos.
    /// </summary>
    public class PaymentSimulationService
    {
        private readonly Random _random = new Random();
        private readonly double _approvalRate;
        private readonly bool _simulationMode;

        public PaymentSimulationService(IConfiguration configuration)
        {
            _simulationMode = configuration.GetValue<bool>("Payment:SimulationMode", true);
            _approvalRate = configuration.GetValue<double>("Payment:ApprovalRate", 0.9);
        }

        /// <summary>
        /// Procesa un pago simulado.
        /// </summary>
        /// <param name="request">Datos de la solicitud de pago</param>
        /// <returns>Resultado del pago simulado</returns>
        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
        {
            // Simular delay de procesamiento (100-500ms)
            await Task.Delay(_random.Next(100, 500));

            // Validar número de tarjeta usando algoritmo de Luhn
            if (!ValidateCardNumber(request.CardNumber))
            {
                return new PaymentResult
                {
                    Success = false,
                    Status = "rejected",
                    Message = "Número de tarjeta inválido",
                    TransactionId = null
                };
            }

            // Validar fecha de expiración
            if (!ValidateExpiryDate(request.ExpiryMonth, request.ExpiryYear))
            {
                return new PaymentResult
                {
                    Success = false,
                    Status = "rejected",
                    Message = "Tarjeta expirada",
                    TransactionId = null
                };
            }

            // Validar CVV
            if (!ValidateCvv(request.Cvv))
            {
                return new PaymentResult
                {
                    Success = false,
                    Status = "rejected",
                    Message = "CVV inválido",
                    TransactionId = null
                };
            }

            // Simular aprobación/rechazo basado en tasa de aprobación
            bool isApproved = _random.NextDouble() < _approvalRate;

            if (isApproved)
            {
                return new PaymentResult
                {
                    Success = true,
                    Status = "approved",
                    Message = "Pago procesado exitosamente",
                    TransactionId = GenerateTransactionId()
                };
            }
            else
            {
                var rejectionReasons = new[]
                {
                    "Fondos insuficientes",
                    "Tarjeta bloqueada por el banco emisor",
                    "Límite de transacciones excedido",
                    "Transacción rechazada por seguridad"
                };

                return new PaymentResult
                {
                    Success = false,
                    Status = "rejected",
                    Message = rejectionReasons[_random.Next(rejectionReasons.Length)],
                    TransactionId = null
                };
            }
        }

        /// <summary>
        /// Valida un número de tarjeta usando el algoritmo de Luhn.
        /// </summary>
        public bool ValidateCardNumber(string cardNumber)
        {
            // Remover espacios y guiones
            cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

            // Debe tener entre 13 y 19 dígitos
            if (cardNumber.Length < 13 || cardNumber.Length > 19)
                return false;

            // Debe contener solo números
            if (!cardNumber.All(char.IsDigit))
                return false;

            // Algoritmo de Luhn
            int sum = 0;
            bool alternate = false;

            for (int i = cardNumber.Length - 1; i >= 0; i--)
            {
                int digit = int.Parse(cardNumber[i].ToString());

                if (alternate)
                {
                    digit *= 2;
                    if (digit > 9)
                        digit -= 9;
                }

                sum += digit;
                alternate = !alternate;
            }

            return (sum % 10 == 0);
        }

        /// <summary>
        /// Valida la fecha de expiración de la tarjeta.
        /// </summary>
        public bool ValidateExpiryDate(string month, string year)
        {
            if (!int.TryParse(month, out int expiryMonth) || !int.TryParse(year, out int expiryYear))
                return false;

            if (expiryMonth < 1 || expiryMonth > 12)
                return false;

            // Si el año tiene 2 dígitos, convertir a 4 dígitos
            if (expiryYear < 100)
                expiryYear += 2000;

            var expiryDate = new DateTime(expiryYear, expiryMonth, 1).AddMonths(1).AddDays(-1);
            return expiryDate >= DateTime.Now;
        }

        /// <summary>
        /// Valida el CVV (debe tener 3 o 4 dígitos).
        /// </summary>
        public bool ValidateCvv(string cvv)
        {
            return cvv.Length >= 3 && cvv.Length <= 4 && cvv.All(char.IsDigit);
        }

        /// <summary>
        /// Genera un ID de transacción simulado.
        /// Formato: TXN-YYYYMMDDHHMMSS-RANDOM
        /// </summary>
        public string GenerateTransactionId()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = _random.Next(1000, 9999);
            return $"TXN-{timestamp}-{random}";
        }

        /// <summary>
        /// Detecta la marca de la tarjeta basándose en el número.
        /// </summary>
        public string DetectCardBrand(string cardNumber)
        {
            cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

            if (cardNumber.StartsWith("4"))
                return "Visa";

            if (cardNumber.StartsWith("5"))
                return "MasterCard";

            if (cardNumber.StartsWith("3"))
            {
                if (cardNumber.StartsWith("34") || cardNumber.StartsWith("37"))
                    return "American Express";
                return "Diners Club";
            }

            if (cardNumber.StartsWith("6"))
                return "Discover";

            return "Unknown";
        }

        /// <summary>
        /// Obtiene los últimos 4 dígitos de una tarjeta.
        /// </summary>
        public string GetLastFourDigits(string cardNumber)
        {
            cardNumber = cardNumber.Replace(" ", "").Replace("-", "");
            return cardNumber.Length >= 4 ? cardNumber.Substring(cardNumber.Length - 4) : cardNumber;
        }
    }

    /// <summary>
    /// DTO para solicitud de pago.
    /// </summary>
    public class PaymentRequest
    {
        public string BookingId { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string CardNumber { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public string ExpiryMonth { get; set; } = string.Empty;
        public string ExpiryYear { get; set; } = string.Empty;
        public string Cvv { get; set; } = string.Empty;
        public string? ConfirmationEmail { get; set; } // Email para enviar confirmaciones (opcional)
    }

    /// <summary>
    /// DTO para resultado de pago.
    /// </summary>
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
    }
}
