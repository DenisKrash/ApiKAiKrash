namespace apikaikrash;

public class ProductDto
{
    public int IdProduct { get; set; }

    public string Article { get; set; } = "";

    public string NameProduct { get; set; } = "";

    public string UnitMeasurement { get; set; } = "";

    public decimal Price { get; set; }

    public int SupplierFk { get; set; }

    public int ManufacturerFk { get; set; }

    public int ProductCategoryFk { get; set; }

    public decimal Discount { get; set; }

    public int QuantityStock { get; set; }

    public string Description { get; set; } = "";

    public string? Photo { get; set; }
}