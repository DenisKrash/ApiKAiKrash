using System;
using System.Collections.Generic;

namespace apikaikrash;

public partial class Manufacturer
{
    public int IdManufacturer { get; set; }

    public string Manufacturer1 { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
