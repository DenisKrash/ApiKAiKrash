using System;
using System.Collections.Generic;

namespace apikaikrash;

public partial class Product
{
    public int IdProduct { get; set; }

    public string Article { get; set; } = null!;

    public string NameProduct { get; set; } = null!;

    public string UnitMeasurement { get; set; } = null!;

    public decimal Price { get; set; }

    public int SupplierFk { get; set; }

    public int ManufacturerFk { get; set; }

    public int ProductCategoryFk { get; set; }

    public decimal Discount { get; set; }

    public int QuantityStock { get; set; }

    public string Description { get; set; } = null!;

    public string? Photo { get; set; }

    public virtual Manufacturer ManufacturerFkNavigation { get; set; } = null!;

    public virtual ProductCategory ProductCategoryFkNavigation { get; set; } = null!;

    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();

    public virtual Supplier SupplierFkNavigation { get; set; } = null!;
}
