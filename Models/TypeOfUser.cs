using System;
using System.Collections.Generic;

namespace Hair_saloon.Models;

public partial class TypeOfUser
{
    public int IdTypeOfUser { get; set; }

    public string RoleOfUser { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
