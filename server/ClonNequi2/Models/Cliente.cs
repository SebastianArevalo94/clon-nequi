using System;
using System.Collections.Generic;

namespace ClonNequi2.Models;

public partial class Cliente
{
    public int Id { get; set; }

    public string Numero { get; set; } = null!;

    public string Nombres { get; set; } = null!;

    public string Apellidos { get; set; } = null!;

    public string Documento { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string Correo { get; set; } = null!;

    public string? Contrasenia { get; set; }

    public virtual ICollection<Cuenta> Cuenta { get; set; } = new List<Cuenta>();

    public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
