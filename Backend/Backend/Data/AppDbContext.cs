using Microsoft.EntityFrameworkCore;
using ExamenBackend.Models;

namespace ExamenBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<CodigoBarra> CodigosBarra { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación entre Producto y CodigoBarra
            modelBuilder.Entity<CodigoBarra>()
                .HasKey(cb => new { cb.ProductoId, cb.Codigo });

            modelBuilder.Entity<CodigoBarra>()
                .HasOne(cb => cb.Producto)
                .WithMany(p => p.CodigosBarra)
                .HasForeignKey(cb => cb.ProductoId);

            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Producto>()
                .Property(p => p.FechaModificacion)
                .IsRequired(false);

            modelBuilder.Entity<CodigoBarra>()
                .Property(cb => cb.FechaModificacion)
                .IsRequired(false);
        }
    }
}
