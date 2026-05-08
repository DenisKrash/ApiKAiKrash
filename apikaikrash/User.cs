using System;
using System.Collections.Generic;

namespace apikaikrash;

public partial class User
{
    public int IdUsers { get; set; }

    public string EmployeeRole { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Patronymic { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
