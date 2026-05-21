using Microsoft.EntityFrameworkCore;
using Orcamento.Data;
using Orcamento.Dtos;
using Orcamento.Models;
using System.ComponentModel;

namespace Orcamento.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _context;
      

        public DashboardService(AppDbContext context)
        {
            _context = context;

        }

        public async Task<CardsDto> BuscarValoresCards(int userId, int? mes = null, int? ano = null)
        {
            int mesRef = mes ?? DateTime.Today.Month;
            int anoRef = ano ?? DateTime.Today.Year;

            DateTime inicio = new DateTime(anoRef, mesRef, 1);
            DateTime fim = new DateTime(anoRef, mesRef + 1, 1);

            var receita = await _context.Transactions
                .Where(t => t.UserId == userId && t.Type == TransactionType.Income &&
                   t.Date >= inicio && t.Date < fim)
                .SumAsync(t => t.Amount);

            var despesa = await _context.Transactions
                .Where(t => t.UserId == userId && t.Type == TransactionType.Expense &&
                    t.Date >= inicio && t.Date < fim)
                .SumAsync(t => t.Amount);

            return new CardsDto
            {
                receita = receita,
                despesa = despesa,
                saldoAtual = receita - despesa
            };
        }
        public async Task<List<GraficoDto>> BuscarValoresGraficoDespesas(int userId, int? mes = null, int? ano = null)
        {

            int mesRef = mes ?? DateTime.Today.Month;
            int anoRef = ano ?? DateTime.Today.Year;

            DateTime inicio = new DateTime(anoRef, mesRef, 1);
            DateTime fim = new DateTime(anoRef, mesRef + 1, 1);

            return await _context.Transactions.Where(
                t => t.UserId == userId && t.Type == TransactionType.Expense &&
                     t.Date >= inicio && t.Date < fim).GroupBy(
                C => C.Category.Name).Select(g => new GraficoDto
                {
                    categoria = g.Key,
                    valor = g.Sum(t => t.Amount) 
                }).OrderByDescending(x => x.valor)
                .Take(5)
                .ToListAsync();
        }
        public async Task<List<GraficoDto>>BuscarValoresGraficoReceitas(int userId, int? mes = null, int? ano = null)
        {
            int mesRef = mes ?? DateTime.Today.Month;
            int anoRef = ano ?? DateTime.Today.Year;

            DateTime inicio = new DateTime(anoRef, mesRef, 1);
            DateTime fim = new DateTime(anoRef, mesRef + 1, 1);

            return await _context.Transactions.Where(
                t => t.UserId == userId && t.Type == TransactionType.Income &&
                    t.Date >= inicio && t.Date < fim).GroupBy(
                C => C.Category.Name).Select(g => new GraficoDto
                {
                    categoria = g.Key,
                    valor = g.Sum(t => t.Amount)
                }).OrderByDescending(x => x.valor)
                .Take(5)
                .ToListAsync();
        }

        public async Task<List<ListaDto>> BuscarValoresListaRecentes(int userId, int? mes = null, int? ano = null)
        {
            int mesRef = mes ?? DateTime.Today.Month;
            int anoRef = ano ?? DateTime.Today.Year;

            DateTime inicio = new DateTime(anoRef, mesRef, 1);
            DateTime fim = new DateTime(anoRef, mesRef + 1, 1);

            return await _context.Transactions
                  .Where(t => t.UserId == userId &&
                    t.Date >= inicio && t.Date < fim)
                  .Select(g => new ListaDto
                  {
                      Title = g.Title,
                      amount = g.Amount,
                      date = g.Date, 
                      categoryName = g.Category.Name,
                  }).OrderByDescending(x => x.date)
                    .Take(5)
                    .ToListAsync();
        }

        public async Task<List<ListaDto>> BuscarValoresListaMaioresTransacoes(int userId, int? mes = null, int? ano = null)
        {
            int mesRef = mes ?? DateTime.Today.Month;
            int anoRef = ano ?? DateTime.Today.Year;

            DateTime inicio = new DateTime(anoRef, mesRef, 1);
            DateTime fim = new DateTime(anoRef, mesRef + 1, 1);

            return await _context.Transactions.Where(t =>
            t.UserId == userId && t.Date >= inicio && t.Date < fim).Select(g => new ListaDto
            {
                Title = g.Title,
                amount = g.Amount,
                date = g.Date,
                categoryName = g.Category.Name,
            }).OrderByDescending(x => x.amount)
                    .Take(5)
                    .ToListAsync();
        }
    }
}
