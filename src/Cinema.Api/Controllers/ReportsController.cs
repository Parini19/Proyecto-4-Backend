using System;
using System.Threading.Tasks;
using Cinema.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ReportService _reportService;

        public ReportsController(ReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.UtcNow.AddDays(-30);
                var end = endDate ?? DateTime.UtcNow;
                var report = await _reportService.GenerateSalesReport(start, end);
                return Ok(new { success = true, report });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to generate sales report.", error = ex.Message });
            }
        }

        [HttpGet("movie-popularity")]
        public async Task<IActionResult> GetMoviePopularityReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.UtcNow.AddDays(-30);
                var end = endDate ?? DateTime.UtcNow;
                var report = await _reportService.GenerateMoviePopularityReport(start, end);
                return Ok(new { success = true, report });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to generate movie popularity report.", error = ex.Message });
            }
        }

        [HttpGet("occupancy")]
        public async Task<IActionResult> GetOccupancyReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.UtcNow.AddDays(-30);
                var end = endDate ?? DateTime.UtcNow;
                var report = await _reportService.GenerateOccupancyReport(start, end);
                return Ok(new { success = true, report });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to generate occupancy report.", error = ex.Message });
            }
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenueReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.UtcNow.AddDays(-30);
                var end = endDate ?? DateTime.UtcNow;
                var report = await _reportService.GenerateRevenueReport(start, end);
                return Ok(new { success = true, report });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to generate revenue report.", error = ex.Message });
            }
        }

        [HttpGet("dashboard-summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            try
            {
                var summary = await _reportService.GenerateDashboardSummary();
                return Ok(new { success = true, summary });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to generate dashboard summary.", error = ex.Message });
            }
        }
    }
}
