using Cinema.Domain.Entities;
using QRCoder;
using System;
using System.IO;

namespace Cinema.Api.Services
{
    /// <summary>
    /// Servicio para generar códigos QR para boletos digitales.
    /// Utiliza la librería QRCoder para crear imágenes QR en formato Base64.
    /// </summary>
    public class QRCodeService
    {
        private readonly int _pixelsPerModule;

        public QRCodeService(IConfiguration configuration)
        {
            // Tamaño del QR (por defecto 300x300 pixels)
            _pixelsPerModule = configuration.GetValue<int>("Tickets:QrCodeSize", 300) / 30;
        }

        /// <summary>
        /// Genera un código QR para un boleto específico.
        /// </summary>
        /// <param name="ticket">Boleto para el cual generar el QR</param>
        /// <returns>String Base64 de la imagen QR</returns>
        public string GenerateQrCodeForTicket(Ticket ticket)
        {
            var qrData = EncodeTicketData(ticket);
            return GenerateQrCodeImage(qrData);
        }

        /// <summary>
        /// Codifica los datos del boleto en un string para el QR.
        /// Formato: TICKET:id=XXX|user=YYY|screening=ZZZ|seat=AAA|showtime=TIMESTAMP
        /// </summary>
        /// <param name="ticket">Boleto a codificar</param>
        /// <returns>String con los datos codificados</returns>
        public string EncodeTicketData(Ticket ticket)
        {
            var showTimeUnix = ((DateTimeOffset)ticket.ShowTime).ToUnixTimeSeconds();
            return $"TICKET:id={ticket.Id}|user={ticket.UserId}|screening={ticket.ScreeningId}|seat={ticket.SeatNumber}|showtime={showTimeUnix}";
        }

        /// <summary>
        /// Decodifica los datos del QR para validación.
        /// </summary>
        /// <param name="qrCodeData">String del QR a decodificar</param>
        /// <returns>Diccionario con los datos decodificados</returns>
        public Dictionary<string, string> DecodeTicketData(string qrCodeData)
        {
            var result = new Dictionary<string, string>();

            if (!qrCodeData.StartsWith("TICKET:"))
            {
                throw new ArgumentException("Invalid QR code format");
            }

            var data = qrCodeData.Substring(7); // Remover "TICKET:"
            var pairs = data.Split('|');

            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    result[keyValue[0]] = keyValue[1];
                }
            }

            return result;
        }

        /// <summary>
        /// Genera la imagen QR en formato Base64 a partir de un string de datos.
        /// </summary>
        /// <param name="data">Datos a codificar en el QR</param>
        /// <returns>String Base64 de la imagen PNG</returns>
        public string GenerateQrCodeImage(string data)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
                using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
                {
                    byte[] qrCodeBytes = qrCode.GetGraphic(_pixelsPerModule);
                    return Convert.ToBase64String(qrCodeBytes);
                }
            }
        }

        /// <summary>
        /// Genera un QR genérico desde cualquier string.
        /// </summary>
        /// <param name="data">Datos a codificar</param>
        /// <returns>String Base64 de la imagen QR</returns>
        public string GenerateQrCode(string data)
        {
            return GenerateQrCodeImage(data);
        }

        /// <summary>
        /// Valida que un string de QR tenga el formato correcto de boleto.
        /// </summary>
        /// <param name="qrCodeData">Datos del QR</param>
        /// <returns>True si el formato es válido</returns>
        public bool ValidateQrCodeFormat(string qrCodeData)
        {
            try
            {
                var decoded = DecodeTicketData(qrCodeData);
                return decoded.ContainsKey("id") &&
                       decoded.ContainsKey("user") &&
                       decoded.ContainsKey("screening") &&
                       decoded.ContainsKey("seat") &&
                       decoded.ContainsKey("showtime");
            }
            catch
            {
                return false;
            }
        }
    }
}
