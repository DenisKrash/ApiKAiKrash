using System;
using System.Collections.Generic;

namespace apikaikrash;

public partial class PickUpPoint
{
    public int IdPickUpPoints { get; set; }

    public int Index { get; set; }

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public int? HouseNumber { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
