using System;
using System.Collections.Generic;

namespace apikaikrash;

public partial class Supplier
{
    public int IdSupplier { get; set; }

    public string Supplier1 { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
