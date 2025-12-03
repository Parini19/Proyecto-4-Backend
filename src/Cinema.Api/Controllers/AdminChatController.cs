using Microsoft.AspNetCore.Mvc;
using Cinema.Api.Services;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/admin/chat")]
    public class AdminChatController : ControllerBase
    {
        private readonly OpenAIChatService _chatService;
        private readonly ReportService _reportService;

        public AdminChatController(OpenAIChatService chatService, ReportService reportService)
        {
            _chatService = chatService;
            _reportService = reportService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AdminChatRequest request)
        {
            try
            {
                // Obtener contexto de reportes y estadísticas
                dynamic dashboardSummary = await _reportService.GenerateDashboardSummary();

                // Construir contexto para el chatbot admin
                var adminContext = $@"
Eres un asistente administrativo del sistema de gestión de cine. Tienes acceso a las siguientes estadísticas actuales:

DASHBOARD:
- Películas totales: {dashboardSummary.totalMovies}
- Funciones totales: {dashboardSummary.totalScreenings}
- Funciones hoy: {dashboardSummary.todayScreenings}
- Combos de comida: {dashboardSummary.totalFoodCombos}
- Reservas totales: {dashboardSummary.totalBookings}
- Reservas hoy: {dashboardSummary.todayBookings}
- Usuarios registrados: {dashboardSummary.totalUsers}
- Ingresos hoy: ₡{dashboardSummary.todayRevenue:N0}

Tu función es ayudar al administrador a:
- Responder preguntas sobre estas estadísticas
- Analizar tendencias y proporcionar insights
- Sugerir acciones basadas en los datos
- Explicar métricas de reportes (ventas, ocupación, popularidad, ingresos)
- Responder preguntas sobre el funcionamiento del sistema

Responde en español, de manera concisa y profesional. Si te preguntan sobre datos que no tienes, indica amablemente que necesitarías esa información específica.
";

                var response = await _chatService.GetChatResponseAsync(request.Message, adminContext);
                return Ok(new { reply = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al procesar la solicitud del chat", details = ex.Message });
            }
        }
    }

    public class AdminChatRequest
    {
        public string Message { get; set; }
    }
}
