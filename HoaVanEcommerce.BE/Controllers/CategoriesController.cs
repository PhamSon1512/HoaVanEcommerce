using HoaVanEcommerce.BE.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HoaVanEcommerce.BE.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    //private readonly ICategoryService _categoryService;

    //public CategoriesController(ICategoryService categoryService)
    //{
    //    _categoryService = categoryService;
    //}

    //[HttpGet]
    //public async Task<IActionResult> GetAll()
    //{
    //    var categories = await _categoryService.GetAllAsync();
    //    return Ok(categories);
    //}
}

