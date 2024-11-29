using System;
using System.Collections.Generic;

namespace ExamenBackend.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int CantidadEnStock { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }

        // Relación con Código de Barra
        public ICollection<CodigoBarra> CodigosBarra { get; set; } = new List<CodigoBarra>();
    }
}
