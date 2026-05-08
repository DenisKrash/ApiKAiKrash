using apikaikrash;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DBController : ControllerBase
{
    private readonly ShoesContext _context;

    public DBController(ShoesContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        string? searchTerm = null,
        bool isSortDescending = false,
        int? supplierId = null)
    {
        var query = _context.Products
            .Include(product => product.SupplierFkNavigation)
            .Include(product => product.ManufacturerFkNavigation)
            .Include(product => product.ProductCategoryFkNavigation)
            .AsQueryable();

        if (supplierId != null)
        {
            query = query.Where(product => product.SupplierFk == supplierId);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string[] words = searchTerm
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                query = query.Where(product =>
                    product.Article.ToLower().Contains(word) ||
                    product.NameProduct.ToLower().Contains(word) ||
                    product.UnitMeasurement.ToLower().Contains(word) ||
                    product.Description.ToLower().Contains(word) ||
                    product.Photo.ToLower().Contains(word) ||
                    product.SupplierFkNavigation.Supplier1.ToLower().Contains(word) ||
                    product.ManufacturerFkNavigation.Manufacturer1.ToLower().Contains(word) ||
                    product.ProductCategoryFkNavigation.ProductCategory1.ToLower().Contains(word));
            }
        }

        if (isSortDescending)
        {
            query = query.OrderByDescending(product => product.QuantityStock);
        }
        else
        {
            query = query.OrderBy(product => product.QuantityStock);
        }

        var products = await query
            .Select(product => new
            {
                product.IdProduct,
                product.Article,
                product.NameProduct,
                product.UnitMeasurement,
                product.Price,
                product.SupplierFk,
                product.ManufacturerFk,
                product.ProductCategoryFk,
                product.Discount,
                product.QuantityStock,
                product.Description,
                product.Photo,

                ManufacturerName = product.ManufacturerFkNavigation.Manufacturer1,
                SupplierName = product.SupplierFkNavigation.Supplier1,
                ProductCategory = product.ProductCategoryFkNavigation.ProductCategory1
            })
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("suppliers")]
    public async Task<IActionResult> GetSuppliers()
    {
        var suppliers = await _context.Suppliers
            .Select(supplier => new
            {
                supplier.IdSupplier,
                supplier.Supplier1
            })
            .ToListAsync();

        return Ok(suppliers);
    }

    [HttpGet("manufacturers")]
    public async Task<IActionResult> GetManufacturers()
    {
        var manufacturers = await _context.Manufacturers
            .Select(manufacturer => new
            {
                manufacturer.IdManufacturer,
                manufacturer.Manufacturer1
            })
            .ToListAsync();

        return Ok(manufacturers);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _context.ProductCategories
            .Select(category => new
            {
                category.IdProductCategory,
                category.ProductCategory1
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _context.Orders
            .Include(order => order.PickUpPointFkNavigation)
            .Include(order => order.ProductOrders)
            .OrderByDescending(order => order.IdOrder)
            .Select(order => new
            {
                order.IdOrder,
                order.DateOrder,
                order.DateDelivery,
                order.PickUpPointFk,
                order.ClientFk,
                order.ReceiptCode,
                order.OrderStatus,

                Article = order.ProductOrders
                    .Select(productOrder => productOrder.ArticleNumberFk)
                    .FirstOrDefault(),

                PickUpPointAddress =
                    order.PickUpPointFkNavigation.Index + ", " +
                    order.PickUpPointFkNavigation.City + ", " +
                    order.PickUpPointFkNavigation.Street + ", " +
                    order.PickUpPointFkNavigation.HouseNumber
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpGet("pickup-points")]
    public async Task<IActionResult> GetPickUpPoints()
    {
        var pickUpPoints = await _context.PickUpPoints
            .Select(point => new
            {
                point.IdPickUpPoints,
                point.Index,
                point.City,
                point.Street,
                point.HouseNumber,

                Address = point.Index + ", " +
                          point.City + ", " +
                          point.Street + ", " +
                          point.HouseNumber
            })
            .ToListAsync();

        return Ok(pickUpPoints);
    }

    [HttpPost("orders")]
    public async Task<IActionResult> AddOrder([FromBody] OrderDto orderDto)
    {
        string? error = CheckOrder(orderDto);

        if (error != null)
        {
            return BadRequest(error);
        }

        bool productExists = await _context.Products
            .AnyAsync(product => product.Article == orderDto.Article);

        if (!productExists)
        {
            return BadRequest("Товар с таким артикулом не найден.");
        }

        int clientId = orderDto.ClientFk;

        if (clientId <= 0)
        {
            clientId = await _context.Orders
                .Select(order => order.ClientFk)
                .FirstOrDefaultAsync();

            if (clientId <= 0)
            {
                return BadRequest("Не найден клиент для заказа.");
            }
        }

        int receiptCode = orderDto.ReceiptCode;

        if (receiptCode <= 0)
        {
            receiptCode = new Random().Next(100, 999);
        }

        Order order = new Order
        {
            DateOrder = orderDto.DateOrder,
            DateDelivery = orderDto.DateDelivery,
            PickUpPointFk = orderDto.PickUpPointFk,
            ClientFk = clientId,
            ReceiptCode = receiptCode,
            OrderStatus = orderDto.OrderStatus!
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        ProductOrder productOrder = new ProductOrder
        {
            OrderNumberFk = order.IdOrder,
            ArticleNumberFk = orderDto.Article!,
            Quantity = 1
        };

        _context.ProductOrders.Add(productOrder);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("orders/{id}")]
    public async Task<IActionResult> EditOrder(int id, [FromBody] OrderDto orderDto)
    {
        string? error = CheckOrder(orderDto);

        if (error != null)
        {
            return BadRequest(error);
        }

        Order? order = await _context.Orders
            .Include(order => order.ProductOrders)
            .FirstOrDefaultAsync(order => order.IdOrder == id);

        if (order == null)
        {
            return NotFound("Заказ не найден.");
        }

        bool productExists = await _context.Products
            .AnyAsync(product => product.Article == orderDto.Article);

        if (!productExists)
        {
            return BadRequest("Товар с таким артикулом не найден.");
        }

        order.DateOrder = orderDto.DateOrder;
        order.DateDelivery = orderDto.DateDelivery;
        order.PickUpPointFk = orderDto.PickUpPointFk;
        order.OrderStatus = orderDto.OrderStatus!;

        if (orderDto.ClientFk > 0)
        {
            order.ClientFk = orderDto.ClientFk;
        }

        if (orderDto.ReceiptCode > 0)
        {
            order.ReceiptCode = orderDto.ReceiptCode;
        }

        ProductOrder? productOrder = order.ProductOrders.FirstOrDefault();

        if (productOrder == null)
        {
            productOrder = new ProductOrder
            {
                OrderNumberFk = order.IdOrder,
                ArticleNumberFk = orderDto.Article!,
                Quantity = 1
            };

            _context.ProductOrders.Add(productOrder);
        }
        else
        {
            productOrder.ArticleNumberFk = orderDto.Article!;
        }

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("orders/{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        Order? order = await _context.Orders
            .Include(order => order.ProductOrders)
            .FirstOrDefaultAsync(order => order.IdOrder == id);

        if (order == null)
        {
            return NotFound("Заказ не найден.");
        }

        _context.ProductOrders.RemoveRange(order.ProductOrders);
        _context.Orders.Remove(order);

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromForm] string productJson, IFormFile? image)
    {
        ProductDto? productDto = JsonSerializer.Deserialize<ProductDto>(productJson);

        if (productDto == null)
        {
            return BadRequest("Данные товара не получены.");
        }

        string? error = CheckProduct(productDto);

        if (error != null)
        {
            return BadRequest(error);
        }

        Product product = new Product
        {
            Article = productDto.Article,
            NameProduct = productDto.NameProduct,
            UnitMeasurement = productDto.UnitMeasurement,
            Price = productDto.Price,
            SupplierFk = productDto.SupplierFk,
            ManufacturerFk = productDto.ManufacturerFk,
            ProductCategoryFk = productDto.ProductCategoryFk,
            Discount = productDto.Discount,
            QuantityStock = productDto.QuantityStock,
            Description = productDto.Description,
            Photo = "picture.png"
        };

        if (image != null)
        {
            product.Photo = await SaveImage(image);
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return Ok(product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditProduct(int id, [FromForm] string productJson, IFormFile? image)
    {
        ProductDto? productDto = JsonSerializer.Deserialize<ProductDto>(productJson);

        if (productDto == null)
        {
            return BadRequest("Данные товара не получены.");
        }

        string? error = CheckProduct(productDto);

        if (error != null)
        {
            return BadRequest(error);
        }

        Product? product = await _context.Products.FirstOrDefaultAsync(p => p.IdProduct == id);

        if (product == null)
        {
            return NotFound("Товар не найден.");
        }

        product.Article = productDto.Article;
        product.NameProduct = productDto.NameProduct;
        product.UnitMeasurement = productDto.UnitMeasurement;
        product.Price = productDto.Price;
        product.SupplierFk = productDto.SupplierFk;
        product.ManufacturerFk = productDto.ManufacturerFk;
        product.ProductCategoryFk = productDto.ProductCategoryFk;
        product.Discount = productDto.Discount;
        product.QuantityStock = productDto.QuantityStock;
        product.Description = productDto.Description;

        if (image != null)
        {
            DeleteOldImage(product.Photo);

            product.Photo = await SaveImage(image);
        }

        await _context.SaveChangesAsync();

        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        Product? product = await _context.Products
            .Include(p => p.ProductOrders)
            .FirstOrDefaultAsync(p => p.IdProduct == id);

        if (product == null)
        {
            return NotFound("Товар не найден.");
        }

        if (product.ProductOrders != null && product.ProductOrders.Any())
        {
            return BadRequest("Нельзя удалить товар, который присутствует в заказе.");
        }

        DeleteOldImage(product.Photo);

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return Ok();
    }

    private string? CheckProduct(ProductDto product)
    {
        if (string.IsNullOrWhiteSpace(product.Article))
        {
            return "Введите артикул товара.";
        }

        if (string.IsNullOrWhiteSpace(product.NameProduct))
        {
            return "Введите наименование товара.";
        }

        if (string.IsNullOrWhiteSpace(product.Description))
        {
            return "Введите описание товара.";
        }

        if (product.Price < 0)
        {
            return "Цена товара не может быть отрицательной.";
        }

        if (product.QuantityStock < 0)
        {
            return "Количество на складе не может быть отрицательным.";
        }

        if (product.Discount < 0)
        {
            return "Скидка не может быть отрицательной.";
        }

        if (product.SupplierFk <= 0)
        {
            return "Выберите поставщика.";
        }

        if (product.ManufacturerFk <= 0)
        {
            return "Выберите производителя.";
        }

        if (product.ProductCategoryFk <= 0)
        {
            return "Выберите категорию товара.";
        }

        return null;
    }

    private string? CheckOrder(OrderDto order)
    {
        if (string.IsNullOrWhiteSpace(order.Article))
        {
            return "Введите артикул.";
        }

        if (string.IsNullOrWhiteSpace(order.OrderStatus))
        {
            return "Выберите статус заказа.";
        }

        if (order.PickUpPointFk <= 0)
        {
            return "Выберите пункт выдачи.";
        }

        if (order.DateOrder == default)
        {
            return "Введите дату заказа.";
        }

        if (order.DateDelivery == default)
        {
            return "Введите дату выдачи.";
        }

        if (order.DateDelivery < order.DateOrder)
        {
            return "Дата выдачи не может быть раньше даты заказа.";
        }

        return null;
    }

    private async Task<string> SaveImage(IFormFile image)
    {
        string imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

        if (!Directory.Exists(imageFolder))
        {
            Directory.CreateDirectory(imageFolder);
        }

        string fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
        string filePath = Path.Combine(imageFolder, fileName);

        using FileStream stream = new FileStream(filePath, FileMode.Create);

        await image.CopyToAsync(stream);

        return fileName;
    }

    private void DeleteOldImage(string? photo)
    {
        if (string.IsNullOrWhiteSpace(photo) || photo == "picture.png")
        {
            return;
        }

        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", photo);

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
    }
}