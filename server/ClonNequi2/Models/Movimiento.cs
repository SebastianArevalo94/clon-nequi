using System;
using System.Collections.Generic;

namespace ClonNequi2.Models;

public partial class Movimiento
{
    public int Id { get; set; }

    public int CuentaId { get; set; }

    public int ClienteId { get; set; }

    public DateTime Fecha { get; set; }

    public double Balance { get; set; }

    public int TipoMovimiento { get; set; }

    //public virtual Cliente Cliente { get; set; } = null!;

    public virtual TipoMovimiento TipoMovimientoNavigation { get; set; } = null!;
}
