using System;
using System.Collections.Generic;

namespace apikaikrash;

public partial class ProductCategory
{
    public int IdProductCategory { get; set; }

    public string ProductCategory1 { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
