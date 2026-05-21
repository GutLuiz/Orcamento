using Microsoft.AspNetCore.Mvc;
using Orcamento.Services;
using System.Security.Claims;

namespace Orcamento.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("cards")]
        public async Task<IActionResult> GetCardsDashboard(int? mes = null, int? ano = null)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var dados = await _dashboardService.BuscarValoresCards(userId,mes,ano);

            return Ok(dados);
        }
        [HttpGet("graficos")]
        public async Task<IActionResult> GetGraficoDashboard(int? mes = null, int? ano = null)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var dadosDespesas = await _dashboardService.BuscarValoresGraficoDespesas(userId,mes,ano);
            var dadosReceitas = await _dashboardService.BuscarValoresGraficoReceitas(userId,mes,ano);

            return Ok(new { dadosDespesas, dadosReceitas });
        }
        [HttpGet("listas")]
        public async Task<IActionResult> GetListaDashboard(int? mes = null, int? ano = null)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var dadosRecentes = await _dashboardService.BuscarValoresListaRecentes(userId,mes,ano);
            var dadosMaiores = await _dashboardService.BuscarValoresListaMaioresTransacoes(userId,mes,ano);

            return Ok(new { dadosRecentes, dadosMaiores });
        }
    }
}