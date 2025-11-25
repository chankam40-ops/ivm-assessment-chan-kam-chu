using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ivm.Assessment.Backend.Data;
using Ivm.Assessment.Backend.Models;

namespace Ivm.Assessment.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly VendingDbContext _db;

    public ProductsController(VendingDbContext db)
    {
        _db = db;
    }

    // GET: api/products
    // [TODO 1]: Return all products from database
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts() {
        // [TODO 1]: here
        return _db.Products;
        throw new NotImplementedException();
    }

    // POST: api/products/purchase
    // [TODO 2]: Implement purchase logic
    // 1.
    // 2. Calulate updated Stock accordingly
    // 3. Create Purchase record and save to database, return success response
    // 4. Api is expected to be throw error when:
        // a. missing request payload
        // b. out of stock + product not found (double check in api layer in addition to frontend)(500 and 404 accordingly with proper msg)
        // c. [BOUNS TODO 3]: call this api again within 5 seconds(To stimulate real machine behaviour)
    // NOTES: you can add functions/variable inside the controller class if need
    [HttpPost("purchase")]
    public async Task<ActionResult<PurchaseResponse>> Purchase([FromBody] PurchaseRequest? request) {
        Thread.Sleep(5000);
        // [TODO 2,3]: here
        if (request == null)
        {
            throw new NotImplementedException("400 Bad Request");
        }
        bool bFound = false;
        bool bOutOfStock = false;
        var Ret_Purchase = new PurchaseResponse();
        foreach (var product_tar in _db.Products)
        {
            //var productToUpdate = await _db.Products.FindAsync(index);
            if (product_tar.Id == request.ProductId)
            {
                if (product_tar.Stock < request.Quantity)
                {
                    bOutOfStock = true;
                    break;
                }
                var Purchases_new = new Purchase();
                Purchases_new.ProductId = product_tar.Id;
                Purchases_new.ProductName = product_tar.Name;
                Purchases_new.Quantity = request.Quantity;
                Purchases_new.Amount = request.Quantity * product_tar.Price;
                Purchases_new.MachineId = "MC_001";
                product_tar.Stock -= Purchases_new.Quantity;
                Ret_Purchase.TotalCost = Purchases_new.Amount;
                Ret_Purchase.QuantityPurchased = Purchases_new.Quantity;
                Ret_Purchase.Remaining = product_tar.Stock;
                _db.Purchases.Add(Purchases_new);
                bFound = true;
                break;
            }
        }
        if (!bFound)
        {
            throw new NotImplementedException("404 Not Found with proper message");
        }
        if (bOutOfStock)
        {
            throw new NotImplementedException("400 Bad Request with proper message");
        }
        await _db.SaveChangesAsync();
        Ret_Purchase.Success = true;
        return Ret_Purchase;
        throw new NotImplementedException();
    }

    // GET: api/products/purchases
    // [TODO 4]: Return all purchases from database
    // [BONUS TODO 5]: Support server side filtering with query parameters
    // Example: ?searchTerm=coke&machineId=machine-001&hours=24&sortField=amount&sortOrder=desc
    [HttpGet("purchases")]
    public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchases([FromQuery] PurchaseFilterParams? filter)
    { 
        // [TODO 4,5]: here
        return _db.Purchases
        throw new NotImplementedException();
    }


    // GET: api/products/balance
    [HttpGet("balance")]
    public ActionResult GetBalance()
    {
        Random random = new Random();
        double randomNumber = random.NextDouble() * 9 + 1;
        double roundedNumber = Math.Round(randomNumber, 2);
        return Ok(new { balance = roundedNumber });
    }
}

public record PurchaseRequest(string ProductId);
