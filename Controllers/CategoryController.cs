using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orcamento.Models;
using Orcamento.Services;
using System.Security.Claims;

[ApiController]
[Route("categories")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost]
    public async Task<IActionResult> CriarCategoria(Category category)
    {
        var result = await _categoryService.CriarCategorias(category, GetUserId());
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> BuscarCategoria()
    {
        var result = await _categoryService.BuscarCategorias(GetUserId());
        return Ok(result);
    }

    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> RemoverCategoria(int categoryId)
    {
        var result = await _categoryService.DeletarCategoria(categoryId, GetUserId());

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{categoryId}")]
    public async Task<IActionResult> AtualizarCategoria(int categoryId, [FromBody] Category categoryName)
    {
        var result = await _categoryService.AtualizarCategoria(categoryId, GetUserId(), categoryName.Name);

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}