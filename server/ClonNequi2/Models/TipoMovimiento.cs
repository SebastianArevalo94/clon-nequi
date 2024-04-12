using System;
using System.Collections.Generic;

namespace ClonNequi2.Models;

public partial class TipoMovimiento
{
    public int Id { get; set; }

    public int Codigo { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
