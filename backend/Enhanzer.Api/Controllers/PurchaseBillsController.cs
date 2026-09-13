using Enhanzer.Api.Data;
using Enhanzer.Api.DTOs;
using Enhanzer.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Enhanzer.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseBillsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PurchaseBillsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] PurchaseBillRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var discountAmount =
            request.StandardCost *
            request.Quantity *
            (request.DiscountPercentage / 100m);

        var totalCost =
            request.StandardCost * request.Quantity -
            discountAmount;

        var totalSelling =
            request.StandardPrice * request.Quantity;

        var purchaseBill = new PurchaseBill
        {
            ItemName = request.ItemName,
            BatchName = request.BatchName,
            StandardCost = request.StandardCost,
            StandardPrice = request.StandardPrice,
            Quantity = request.Quantity,
            DiscountPercentage = request.DiscountPercentage,
            TotalCost = totalCost,
            TotalSelling = totalSelling
        };

        _context.PurchaseBills.Add(purchaseBill);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Purchase bill item added successfully.",
            data = new
            {
                purchaseBill.Id,
                purchaseBill.ItemName,
                purchaseBill.BatchName,
                purchaseBill.StandardCost,
                purchaseBill.StandardPrice,
                purchaseBill.Quantity,
                purchaseBill.DiscountPercentage,
                purchaseBill.TotalCost,
                purchaseBill.TotalSelling
            }
        });
    }
}