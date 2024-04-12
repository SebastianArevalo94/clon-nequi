using System;
using System.Collections.Generic;

namespace ClonNequi2.Models;

public partial class Cuenta
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public double Balance { get; set; }

    //public virtual Cliente Cliente { get; set; } = null!;
}
