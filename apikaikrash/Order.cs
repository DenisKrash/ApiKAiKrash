using System;
using System.Collections.Generic;

namespace apikaikrash;

public partial class Order
{
    public int IdOrder { get; set; }

    public DateOnly DateOrder { get; set; }

    public DateOnly DateDelivery { get; set; }

    public int PickUpPointFk { get; set; }

    public int ClientFk { get; set; }

    public int ReceiptCode { get; set; }

    public string OrderStatus { get; set; } = null!;

    public virtual User ClientFkNavigation { get; set; } = null!;

    public virtual PickUpPoint PickUpPointFkNavigation { get; set; } = null!;

    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();
}
