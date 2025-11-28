using Microsoft.AspNetCore.Mvc;
using Cinema.Api.Services;
using Serilog;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/cleanup")]
    public class DatabaseCleanupController : ControllerBase
    {
        private readonly FirestoreScreeningService _screeningService;
        private readonly FirestoreBookingService _bookingService;
        private readonly FirestorePaymentService _paymentService;
        private readonly FirestoreTicketService _ticketService;
        private readonly FirestoreInvoiceService _invoiceService;
        private readonly FirestoreFoodOrderService _foodOrderService;
        private readonly FirestoreAuditLogService _auditLogService;

        public DatabaseCleanupController(
            FirestoreScreeningService screeningService,
            FirestoreBookingService bookingService,
            FirestorePaymentService paymentService,
            FirestoreTicketService ticketService,
            FirestoreInvoiceService invoiceService,
            FirestoreFoodOrderService foodOrderService,
            FirestoreAuditLogService auditLogService)
        {
            _screeningService = screeningService;
            _bookingService = bookingService;
            _paymentService = paymentService;
            _ticketService = ticketService;
            _invoiceService = invoiceService;
            _foodOrderService = foodOrderService;
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// DANGER: Deletes ALL transactional data (bookings, payments, tickets, invoices, food orders, screenings, audit logs).
        /// Theater rooms and cinema locations are preserved.
        /// POST /api/cleanup/clear-all-data
        /// </summary>
        [HttpPost("clear-all-data")]
        public async Task<IActionResult> ClearAllData()
        {
            try
            {
                var stats = new
                {
                    screeningsDeleted = 0,
                    bookingsDeleted = 0,
                    paymentsDeleted = 0,
                    ticketsDeleted = 0,
                    invoicesDeleted = 0,
                    foodOrdersDeleted = 0,
                    auditLogsDeleted = 0
                };

                Log.Warning("🗑️ Starting complete database cleanup...");

                // 1. Delete Audit Logs
                try
                {
                    var auditLogs = await _auditLogService.GetAllAuditLogsAsync();
                    foreach (var log in auditLogs)
                    {
                        await _auditLogService.DeleteAuditLogAsync(log.Id);
                        stats = stats with { auditLogsDeleted = stats.auditLogsDeleted + 1 };
                    }
                    Log.Information("Deleted {Count} audit logs", stats.auditLogsDeleted);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to delete audit logs");
                }

                // 2. Delete Food Orders
                try
                {
                    var foodOrders = await _foodOrderService.GetAllFoodOrdersAsync();
                    foreach (var order in foodOrders)
                    {
                        await _foodOrderService.DeleteFoodOrderAsync(order.Id);
                        stats = stats with { foodOrdersDeleted = stats.foodOrdersDeleted + 1 };
                    }
                    Log.Information("Deleted {Count} food orders", stats.foodOrdersDeleted);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to delete food orders");
                }

                // 3. Delete Invoices
                try
                {
                    var invoices = await _invoiceService.GetAllInvoicesAsync();
                    foreach (var invoice in invoices)
                    {
                        await _invoiceService.DeleteInvoiceAsync(invoice.Id);
                        stats = stats with { invoicesDeleted = stats.invoicesDeleted + 1 };
                    }
                    Log.Information("Deleted {Count} invoices", stats.invoicesDeleted);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to delete invoices");
                }

                // 4. Delete Tickets
                try
                {
                    var tickets = await _ticketService.GetAllTicketsAsync();
                    foreach (var ticket in tickets)
                    {
                        await _ticketService.DeleteTicketAsync(ticket.Id);
                        stats = stats with { ticketsDeleted = stats.ticketsDeleted + 1 };
                    }
                    Log.Information("Deleted {Count} tickets", stats.ticketsDeleted);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to delete tickets");
                }

                // 5. Delete Payments
                try
                {
                    var payments = await _paymentService.GetAllPaymentsAsync();
                    foreach (var payment in payments)
                    {
                        await _paymentService.DeletePaymentAsync(payment.Id);
                        stats = stats with { paymentsDeleted = stats.paymentsDeleted + 1 };
                    }
                    Log.Information("Deleted {Count} payments", stats.paymentsDeleted);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to delete payments");
                }

                // 6. Delete Bookings
                try
                {
                    var bookings = await _bookingService.GetAllBookingsAsync();
                    foreach (var booking in bookings)
                    {
                        await _bookingService.DeleteBookingAsync(booking.Id);
                        stats = stats with { bookingsDeleted = stats.bookingsDeleted + 1 };
                    }
                    Log.Information("Deleted {Count} bookings", stats.bookingsDeleted);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to delete bookings");
                }

                // 7. Delete Screenings (last, since bookings depend on them)
                try
                {
                    var screenings = await _screeningService.GetAllScreeningsAsync();
                    foreach (var screening in screenings)
                    {
                        await _screeningService.DeleteScreeningAsync(screening.Id);
                        stats = stats with { screeningsDeleted = stats.screeningsDeleted + 1 };
                    }
                    Log.Information("Deleted {Count} screenings", stats.screeningsDeleted);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to delete screenings");
                }

                Log.Warning("✅ Database cleanup completed successfully");

                return Ok(new
                {
                    success = true,
                    message = "All transactional data has been deleted successfully",
                    statistics = stats,
                    totalDeleted = stats.screeningsDeleted + stats.bookingsDeleted + stats.paymentsDeleted +
                                   stats.ticketsDeleted + stats.invoicesDeleted + stats.foodOrdersDeleted + stats.auditLogsDeleted,
                    note = "Theater rooms, cinemas, movies, and food combos are preserved"
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fatal error during database cleanup");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Error during cleanup: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Deletes only old screenings (older than today).
        /// POST /api/cleanup/clear-old-screenings
        /// </summary>
        [HttpPost("clear-old-screenings")]
        public async Task<IActionResult> ClearOldScreenings()
        {
            try
            {
                var allScreenings = await _screeningService.GetAllScreeningsAsync();
                var today = DateTime.UtcNow.Date;
                var oldScreenings = allScreenings.Where(s => s.StartTime.Date < today).ToList();

                int deletedCount = 0;
                foreach (var screening in oldScreenings)
                {
                    await _screeningService.DeleteScreeningAsync(screening.Id);
                    deletedCount++;
                }

                return Ok(new
                {
                    success = true,
                    message = $"Deleted {deletedCount} old screenings",
                    deletedCount,
                    remainingScreenings = allScreenings.Count - deletedCount
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = $"Error deleting old screenings: {ex.Message}"
                });
            }
        }
    }
}
