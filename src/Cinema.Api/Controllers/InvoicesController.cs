using System;
using System.Threading.Tasks;
using Cinema.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers
{
    /// <summary>
    /// Controlador para gestionar facturas.
    /// Permite consultar facturas y descargar PDFs.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly FirestoreInvoiceService _invoiceService;
        private readonly InvoiceService _invoiceBusinessService;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(
            FirestoreInvoiceService invoiceService,
            InvoiceService invoiceBusinessService,
            ILogger<InvoicesController> logger)
        {
            _invoiceService = invoiceService;
            _invoiceBusinessService = invoiceBusinessService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene una factura por ID.
        /// GET /api/invoices/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(string id)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceAsync(id);
                if (invoice == null)
                    return NotFound(new { success = false, message = "Invoice not found" });

                return Ok(new { success = true, invoice });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting invoice {id}");
                return StatusCode(500, new { success = false, message = "Failed to get invoice", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene la factura asociada a una reserva.
        /// GET /api/invoices/booking/{bookingId}
        /// </summary>
        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetInvoiceByBooking(string bookingId)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceByBookingIdAsync(bookingId);
                if (invoice == null)
                    return NotFound(new { success = false, message = "Invoice not found for this booking" });

                return Ok(new { success = true, invoice });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting invoice for booking {bookingId}");
                return StatusCode(500, new { success = false, message = "Failed to get invoice", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todas las facturas de un usuario.
        /// GET /api/invoices/user/{userId}
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserInvoices(string userId)
        {
            try
            {
                var invoices = await _invoiceService.GetInvoicesByUserIdAsync(userId);
                return Ok(new { success = true, invoices, count = invoices.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting invoices for user {userId}");
                return StatusCode(500, new { success = false, message = "Failed to get invoices", error = ex.Message });
            }
        }

        /// <summary>
        /// Busca una factura por su número.
        /// GET /api/invoices/number/{invoiceNumber}
        /// </summary>
        [HttpGet("number/{invoiceNumber}")]
        public async Task<IActionResult> GetInvoiceByNumber(string invoiceNumber)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceByNumberAsync(invoiceNumber);
                if (invoice == null)
                    return NotFound(new { success = false, message = "Invoice not found" });

                return Ok(new { success = true, invoice });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting invoice by number {invoiceNumber}");
                return StatusCode(500, new { success = false, message = "Failed to get invoice", error = ex.Message });
            }
        }

        /// <summary>
        /// Descarga una factura en formato PDF.
        /// GET /api/invoices/{id}/download
        /// </summary>
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadInvoice(string id)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceAsync(id);
                if (invoice == null)
                    return NotFound(new { success = false, message = "Invoice not found" });

                // Generar PDF
                var pdfBytes = _invoiceBusinessService.GenerateInvoicePdf(invoice);

                var fileName = $"Invoice-{invoice.InvoiceNumber}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading invoice {id}");
                return StatusCode(500, new { success = false, message = "Failed to download invoice", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene todas las facturas (solo para admins).
        /// GET /api/invoices/all
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllInvoices()
        {
            try
            {
                var invoices = await _invoiceService.GetAllInvoicesAsync();
                return Ok(new { success = true, invoices, count = invoices.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all invoices");
                return StatusCode(500, new { success = false, message = "Failed to get invoices", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene facturas emitidas en un rango de fechas.
        /// GET /api/invoices/range?startDate=2025-01-01&endDate=2025-01-31
        /// </summary>
        [HttpGet("range")]
        public async Task<IActionResult> GetInvoicesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (endDate < startDate)
                    return BadRequest(new { success = false, message = "End date must be after start date" });

                var invoices = await _invoiceService.GetInvoicesByDateRangeAsync(startDate, endDate);
                return Ok(new { success = true, invoices, count = invoices.Count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting invoices by date range");
                return StatusCode(500, new { success = false, message = "Failed to get invoices", error = ex.Message });
            }
        }
    }
}
