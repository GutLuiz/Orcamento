using Microsoft.EntityFrameworkCore;
using Orcamento.Data;
using Orcamento.Models;

namespace Orcamento.Services
{
    public class CategoryService 
    {
        private readonly AppDbContext _context;
        private readonly DateTime _inicioMesAtual;
        private readonly DateTime _hoje;

        public CategoryService(AppDbContext context)
        {
            _context = context;
            _inicioMesAtual = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            _hoje = DateTime.Now;
        }

        public async Task<Category?> CriarCategorias(Category category, int userId)
        {
            category.UserId = userId;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<List<Category?>> BuscarCategorias(int userId)
        {
            var categories = _context.Categories
               .Where(c => c.UserId == userId)
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    MovimentacaoMensal = _context.Transactions
               .Where(t => t.UserId == userId && t.CategoryId == c.Id &&
                    t.Date >= _inicioMesAtual)
               .Sum(t => (decimal?)t.Amount) ?? 0
                })
               .ToList();
            return categories;
        }

        public async Task<Category?> DeletarCategoria(int categoryId, int userId)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c =>
            c.Id == categoryId && c.UserId == userId);

            if(category == null)
            {
                return null;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<Category?> AtualizarCategoria(int categoryId, int userId, string categoryName)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId
            && c.UserId == userId);

            if(category == null)
            {
                return null;
            }

            category.Name = categoryName;

            await _context.SaveChangesAsync();

            return category;
        }
    }
}
