using Zadanie9.Model.Requests;
using Zadanie9.Services;

namespace Zadanie9.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly IDbService _dbService;

    public WarehouseController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpPost("add-product")]
    public async Task<IActionResult> AddProduct([FromBody] AddProductRequest request)
    {
        try
        {
            var id = await _dbService.AddProductToWarehouse(request);
            return Ok(new { id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("add-product-procedure")]
    public async Task<IActionResult> AddProductUsingProcedure([FromBody] AddProductRequest request)
    {
        try
        {
            var id = await _dbService.AddProductToWarehouseUsingProcedure(request);
            return Ok(new { id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}