using System;
using System.Collections.Generic;

namespace apikaikrash;

public class OrderDto
{
    public int IdOrder { get; set; }

    public string? Article { get; set; }

    public DateOnly DateOrder { get; set; }

    public DateOnly DateDelivery { get; set; }

    public int PickUpPointFk { get; set; }

    public int ClientFk { get; set; }

    public int ReceiptCode { get; set; }

    public string? OrderStatus { get; set; }
}