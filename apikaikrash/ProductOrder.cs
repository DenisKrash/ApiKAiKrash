using System;
using System.Collections.Generic;

namespace apikaikrash;

public partial class ProductOrder
{
    public int IdProductOrder { get; set; }

    public int OrderNumberFk { get; set; }

    public string ArticleNumberFk { get; set; } = null!;

    public int Quantity { get; set; }

    public virtual Product ArticleNumberFkNavigation { get; set; } = null!;

    public virtual Order OrderNumberFkNavigation { get; set; } = null!;
}
