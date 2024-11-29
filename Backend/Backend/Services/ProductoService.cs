using Backend.DTOs;
using ExamenBackend.Data;
using ExamenBackend.Models;
using ExamenBackend.Repositories.Interfaces;
using ExamenBackend.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ExamenBackend.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _repository;

        public ProductoService(IProductoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductoDto> GetProductoByIdAsync(int id)
        {
            var producto = await _repository.GetProductoByIdAsync(id);
            if (producto == null) throw new Exception("Producto no encontrado.");
            return MapProductoToDto(producto);
        }

        public async Task<IEnumerable<ProductoDto>> GetAllProductosAsync()
        {
            var productos = await _repository.GetAllProductosAsync();
            return productos.Select(MapProductoToDto);
        }

        public async Task<ProductoDto> CreateProductoAsync(ProductoDto productoDto)
        {
            try
            {
                var producto = MapDtoToProducto(productoDto);
                var createdProducto = await _repository.AddProductoAsync(producto);
                return MapProductoToDto(createdProducto);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2601)
                {
                    throw new Exception("El código de barra proporcionado ya existe. Por favor, use un código único.");
                }
                else
                {
                    throw new Exception("Hubo un error al guardar el producto. Intente nuevamente.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al intentar crear el producto: " + ex.Message);
            }
        }

        public async Task<ProductoDto> UpdateProductoAsync(ProductoDto productoDto)
        {
            try
            {
                if (productoDto.Id <= 0)
                {
                    throw new ArgumentException("El ID del producto no es válido.");
                }

                // Mapeamos el DTO a Producto
                var producto = MapDtoToProducto(productoDto);

                // Obtenemos el producto existente desde el repositorio
                var productoExistente = await _repository.GetProductoByIdAsync(producto.Id);
                if (productoExistente == null)
                {
                    throw new Exception("Producto no encontrado.");
                }

                // Actualizamos las propiedades del producto existente con los nuevos valores del DTO
                productoExistente.Nombre = producto.Nombre;
                productoExistente.Precio = producto.Precio;
                productoExistente.CantidadEnStock = producto.CantidadEnStock;
                productoExistente.Activo = producto.Activo;
                productoExistente.FechaModificacion = DateTime.UtcNow;

                // Actualizamos la entidad sin adjuntar otra instancia
                var updatedProducto = await _repository.UpdateProductoAsync(productoExistente);
                return MapProductoToDto(updatedProducto);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
                {
                    throw new Exception("El código de barra proporcionado ya existe. No se pueden duplicar los valores.");
                }
                else
                {
                    throw new Exception("Hubo un error al intentar actualizar el producto. Intente nuevamente.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un error al intentar actualizar el producto: " + ex.Message);
            }
        }

        public async Task<bool> DeleteProductoAsync(int id)
        {
            return await _repository.DeleteProductoAsync(id);
        }

        // Métodos de mapeo
        private ProductoDto MapProductoToDto(Producto producto)
        {
            return new ProductoDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                Precio = producto.Precio,
                CantidadEnStock = producto.CantidadEnStock,
                Activo = producto.Activo,
                FechaAlta = producto.FechaAlta,
                FechaModificacion = producto.FechaModificacion,
                CodigosBarra = producto.CodigosBarra.Select(cb => new CodigoBarraDto
                {
                    ProductoId = cb.ProductoId,
                    Codigo = cb.Codigo,
                    Activo = cb.Activo,
                    FechaAlta = cb.FechaAlta,
                    FechaModificacion = cb.FechaModificacion
                }).ToList()
            };
        }

        private Producto MapDtoToProducto(ProductoDto productoDto)
        {
            return new Producto
            {
                Id = productoDto.Id,
                Nombre = productoDto.Nombre,
                Precio = productoDto.Precio,
                CantidadEnStock = productoDto.CantidadEnStock,
                Activo = productoDto.Activo,
                FechaAlta = productoDto.FechaAlta,
                FechaModificacion = productoDto.FechaModificacion,
                CodigosBarra = productoDto.CodigosBarra.Select(cbDto => new CodigoBarra
                {
                    Codigo = cbDto.Codigo,
                    Activo = cbDto.Activo,
                    FechaAlta = cbDto.FechaAlta,
                    FechaModificacion = cbDto.FechaModificacion
                }).ToList()
            };
        }
    }
}
