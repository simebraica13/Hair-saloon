using System;
using System.Collections.Generic;

namespace Hair_saloon.Models;

public partial class User
{
    public int UserId { get; set; }

    public int? TypeOfUserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual TypeOfUser? TypeOfUser { get; set; }
}
