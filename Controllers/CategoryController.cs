using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orcamento.Data;
using Orcamento.Models;
using Orcamento.Services;
using System.Security.Claims;

namespace Orcamento.Controllers
{
    [ApiController]
    [Route("categories")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        
        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [HttpPost]
        public async Task<IActionResult> CriarCategoria(Category category)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _categoryService.CriarCategorias(category, userId);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarCategoria()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _categoryService.BuscarCategorias(userId);

            return Ok(result);
        }

        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> RemoverCategoria(int categoryId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _categoryService.DeletarCategoria(categoryId, userId);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPut("{categoryId}")]
        public async Task<IActionResult> AtualizarCategoria(int categoryId,[FromBody] Category categoryName)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _categoryService.AtualizarCategoria(categoryId, userId, categoryName.Name);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}