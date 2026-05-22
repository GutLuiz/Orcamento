using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orcamento.Models;
using Orcamento.Services;
using System.Security.Claims;

namespace Orcamento.Controllers
{
    [ApiController]
    [Route("transactions")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly TransacoesService _transacoesService;

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public TransactionController(TransacoesService transacoesService)
        {
            _transacoesService = transacoesService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarTransacao(Transaction transaction)
        {
            var result = await _transacoesService.CriarTransacoes(transaction, GetUserId());
           
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Buscartransacao(int? mes = null, int? ano = null)
        {
            var result = await _transacoesService.BuscarTransacoes(GetUserId(), mes, ano);

            return Ok(result);
        }
        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> DeletarCategoria(int transactionId)
        {
            var result = await _transacoesService.DeletarCategorias(transactionId, GetUserId());

            if (result == null)
            {
                return NotFound();
            }
            
            return NoContent();
        }
        [HttpPut("{transactionId}")]
        public async Task<IActionResult> Update(int transactionId, Transaction transactions)
        {
            var result = await _transacoesService.AtualizarTransacoes(
                transactionId, GetUserId(), transactions);
           
            if (result == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
