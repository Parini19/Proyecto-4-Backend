using Cinema.Api.Services;
using Cinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Api.Services
{
    public class ReportService
    {
        private readonly FirestoreMovieService _movieService;
        private readonly FirestoreScreeningService _screeningService;
        private readonly FirestoreFoodComboService _foodComboService;
        private readonly FirestoreBookingService _bookingService;
        private readonly FirestoreUserService _userService;

        public ReportService(
            FirestoreMovieService movieService,
            FirestoreScreeningService screeningService,
            FirestoreFoodComboService foodComboService,
            FirestoreBookingService bookingService,
            FirestoreUserService userService)
        {
            _movieService = movieService;
            _screeningService = screeningService;
            _foodComboService = foodComboService;
            _bookingService = bookingService;
            _userService = userService;
        }

        public async Task<object> GenerateSalesReport(DateTime startDate, DateTime endDate)
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            var filteredBookings = bookings.Where(b =>
                b.CreatedAt >= startDate && b.CreatedAt <= endDate).ToList();

            var totalSales = filteredBookings.Sum(b => b.Total);
            var totalBookings = filteredBookings.Count;
            var avgBookingValue = totalBookings > 0 ? totalSales / totalBookings : 0;

            return new
            {
                reportType = "Sales",
                startDate,
                endDate,
                totalSales,
                totalBookings,
                averageBookingValue = avgBookingValue,
                dailyBreakdown = filteredBookings
                    .GroupBy(b => b.CreatedAt.Date)
                    .Select(g => new { date = g.Key, sales = g.Sum(b => b.Total), count = g.Count() })
                    .OrderBy(x => x.date)
                    .ToList()
            };
        }

        public async Task<object> GenerateMoviePopularityReport(DateTime startDate, DateTime endDate)
        {
            var movies = await _movieService.GetAllMoviesAsync();
            var bookings = await _bookingService.GetAllBookingsAsync();
            var screenings = await _screeningService.GetAllScreeningsAsync();

            // Create lookup from ScreeningId to MovieId
            var screeningToMovie = screenings.ToDictionary(s => s.Id, s => s.MovieId);

            var movieStats = movies.Select(m => new
            {
                movieId = m.Id,
                title = m.Title,
                bookings = bookings.Count(b =>
                    screeningToMovie.ContainsKey(b.ScreeningId) &&
                    screeningToMovie[b.ScreeningId] == m.Id &&
                    b.CreatedAt >= startDate && b.CreatedAt <= endDate),
                revenue = bookings.Where(b =>
                    screeningToMovie.ContainsKey(b.ScreeningId) &&
                    screeningToMovie[b.ScreeningId] == m.Id &&
                    b.CreatedAt >= startDate && b.CreatedAt <= endDate)
                    .Sum(b => b.Total)
            })
            .OrderByDescending(x => x.bookings)
            .Take(10)
            .ToList();

            return new
            {
                reportType = "Movie Popularity",
                startDate,
                endDate,
                topMovies = movieStats
            };
        }

        public async Task<object> GenerateOccupancyReport(DateTime startDate, DateTime endDate)
        {
            var screenings = await _screeningService.GetAllScreeningsAsync();
            var filteredScreenings = screenings.Where(s =>
                s.StartTime >= startDate && s.StartTime <= endDate).ToList();

            var totalScreenings = filteredScreenings.Count;
            var averageOccupancy = totalScreenings > 0 ?
                new Random().Next(60, 90) : 0; // Simulado - en producción calcular real

            return new
            {
                reportType = "Occupancy",
                startDate,
                endDate,
                totalScreenings,
                averageOccupancyRate = averageOccupancy,
                screeningsByDay = filteredScreenings
                    .GroupBy(s => s.StartTime.Date)
                    .Select(g => new { date = g.Key, count = g.Count() })
                    .OrderBy(x => x.date)
                    .ToList()
            };
        }

        public async Task<object> GenerateRevenueReport(DateTime startDate, DateTime endDate)
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            var filteredBookings = bookings.Where(b =>
                b.CreatedAt >= startDate && b.CreatedAt <= endDate).ToList();

            var ticketRevenue = filteredBookings.Sum(b => b.TicketPrice * b.TicketQuantity);
            var totalRevenue = filteredBookings.Sum(b => b.Total);
            var foodRevenue = totalRevenue - ticketRevenue;

            return new
            {
                reportType = "Revenue",
                startDate,
                endDate,
                totalRevenue,
                ticketRevenue,
                foodRevenue,
                breakdown = new
                {
                    tickets = new { revenue = ticketRevenue, percentage = totalRevenue > 0 ? (ticketRevenue / totalRevenue) * 100 : 0 },
                    food = new { revenue = foodRevenue, percentage = totalRevenue > 0 ? (foodRevenue / totalRevenue) * 100 : 0 }
                }
            };
        }

        public async Task<object> GenerateDashboardSummary()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            var screenings = await _screeningService.GetAllScreeningsAsync();
            var foodCombos = await _foodComboService.GetAllFoodCombosAsync();
            var bookings = await _bookingService.GetAllBookingsAsync();
            var users = await _userService.GetAllUsersAsync();

            var today = DateTime.UtcNow.Date;
            var todayScreenings = screenings.Where(s => s.StartTime.Date == today).ToList();
            var todayBookings = bookings.Where(b => b.CreatedAt.Date == today).ToList();

            return new
            {
                totalMovies = movies.Count,
                totalScreenings = screenings.Count,
                todayScreenings = todayScreenings.Count,
                totalFoodCombos = foodCombos.Count,
                totalBookings = bookings.Count,
                todayBookings = todayBookings.Count,
                totalUsers = users.Count,
                todayRevenue = todayBookings.Sum(b => b.Total)
            };
        }
    }
}
