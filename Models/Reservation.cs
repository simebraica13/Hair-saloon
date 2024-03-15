using System;
using System.Collections.Generic;

namespace Hair_saloon.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int? UserId { get; set; }

    public DateOnly ReservationDate { get; set; }

    public bool Payed { get; set; }

    public virtual User? User { get; set; }
}
