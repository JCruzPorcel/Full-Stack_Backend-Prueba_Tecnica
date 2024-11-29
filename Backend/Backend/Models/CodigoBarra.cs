using System;

namespace ExamenBackend.Models
{
    public class CodigoBarra
    {
        public int ProductoId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // Relación con Producto
        public Producto? Producto { get; set; }
    }
}
