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

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("cards")]
        public async Task<IActionResult> GetCardsDashboard(int? mes = null, int? ano = null)
        {
            var dados = await _dashboardService.BuscarValoresCards(GetUserId(), mes,ano);

            return Ok(dados);
        }
        [HttpGet("graficos")]
        public async Task<IActionResult> GetGraficoDashboard(int? mes = null, int? ano = null)
        {
            var dadosDespesas = await _dashboardService.BuscarValoresGraficoDespesas(GetUserId(), mes,ano);
            var dadosReceitas = await _dashboardService.BuscarValoresGraficoReceitas(GetUserId(), mes,ano);

            return Ok(new { dadosDespesas, dadosReceitas });
        }
        [HttpGet("listas")]
        public async Task<IActionResult> GetListaDashboard(int? mes = null, int? ano = null)
        {
            var dadosRecentes = await _dashboardService.BuscarValoresListaRecentes(GetUserId(), mes,ano);
            var dadosMaiores = await _dashboardService.BuscarValoresListaMaioresTransacoes(GetUserId(), mes,ano);

            return Ok(new { dadosRecentes, dadosMaiores });
        }
    }
}