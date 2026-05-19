using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orcamento.Data;
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

        public TransactionController(TransacoesService transacoesService)
        {
            _transacoesService = transacoesService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarTransacao(Transaction transaction)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _transacoesService.CriarTransacoes(transaction,userId);
           
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Buscartransacao(int? mes = null, int? ano = null)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _transacoesService.BuscarTransacoes(userId, mes, ano);

            return Ok(result);
        }
        [HttpDelete("{transactionId}")]
        public async Task<IActionResult> DeletarCategoria(int transactionId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _transacoesService.DeletarCategorias(transactionId, userId);

            if (result == null)
            {
                return NotFound();
            }
            
            return NoContent();
        }
        [HttpPut("{transactionId}")]
        public async Task<IActionResult> Update(int transactionId, Transaction transactions)
        {
            var userId = int.Parse(
               User.FindFirst(ClaimTypes.NameIdentifier)!.Value
             );

            var result = await _transacoesService.AtualizarTransacoes(
                transactionId, userId, transactions);
           
            if (result == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
