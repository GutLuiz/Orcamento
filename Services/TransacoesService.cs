using Microsoft.EntityFrameworkCore;
using Orcamento.Data;
using Orcamento.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Orcamento.Services
{
    public class TransacoesService
    {
        private readonly AppDbContext _context;
        private readonly DateTime _inicioMesAtual;
        private readonly DateTime _hoje;

        public TransacoesService(AppDbContext context)
        {
            _context = context;
            _inicioMesAtual = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            _hoje = DateTime.Now;
        }

        public async Task<Transaction?> CriarTransacoes(Transaction transaction, int userId)
        {
            var category = _context.Categories
              .FirstOrDefault(c => c.Id == transaction.CategoryId && c.UserId == userId);

            if (category == null)
            {
                return null;
            }

            transaction.UserId = userId;

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<List<Transaction?>> BuscarTransacoes(int userId, int? mes = null, int? ano = null)
        {
            var query = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId);

            if (mes.HasValue && ano.HasValue)
            {
                query = query.Where(t =>
                    t.Date.Month == mes.Value &&
                    t.Date.Year == ano.Value);
            }

            if(query == null)
            {
                return null;
            }

            return await query.ToListAsync();
        }

        public async Task<Transaction?> DeletarCategorias(int transactionId, int userId)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t =>
            t.Id == transactionId && t.UserId == userId);

            if(transaction == null)
            {
                return null;
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction?> AtualizarTransacoes(
            int transactionId, int userId, Transaction transactions)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(t =>
                t.Id == transactionId && t.UserId == userId);

            if(transaction == null)
            {
                return null;
            }

            transaction.Title = transactions.Title;
            transaction.Amount = transactions.Amount;
            transaction.Type = transactions.Type;
            transaction.Date = transactions.Date;
            transaction.CategoryId = transactions.CategoryId;

            await _context.SaveChangesAsync();

            return transaction;
        }
    }
}
