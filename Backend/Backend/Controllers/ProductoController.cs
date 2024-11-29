using Backend.DTOs;
using ExamenBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExamenBackend.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        // Obtener todos los productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDto>>> GetProductos()
        {
            try
            {
                var productos = await _productoService.GetAllProductosAsync();
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Hubo un problema al obtener los productos.", error = ex.Message });
            }
        }

        // Crear un nuevo Producto
        [HttpPost]
        public async Task<ActionResult<ProductoDto>> CreateProducto([FromBody] ProductoDto productoDto)
        {
            if (productoDto == null)
            {
                return BadRequest("El producto no puede ser nulo.");
            }

            try
            {
                var producto = await _productoService.CreateProductoAsync(productoDto);
                return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Hubo un problema al crear el producto.", error = ex.Message });
            }
        }

        // Actualizar un Producto existente
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductoDto>> UpdateProducto(int id, [FromBody] ProductoDto productoDto)
        {
            if (id != productoDto.Id)
            {
                return BadRequest("El ID del producto no coincide con el proporcionado en la solicitud.");
            }

            try
            {
                var producto = await _productoService.UpdateProductoAsync(productoDto);
                return Ok(producto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Hubo un problema al actualizar el producto.", error = ex.Message });
            }
        }

        // Obtener un Producto por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDto>> GetProducto(int id)
        {
            try
            {
                var producto = await _productoService.GetProductoByIdAsync(id);
                if (producto == null)
                {
                    return NotFound(new { message = "Producto no encontrado." });
                }
                return Ok(producto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Hubo un problema al obtener el producto.", error = ex.Message });
            }
        }

        // Eliminar un Producto
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProducto(int id)
        {
            try
            {
                var result = await _productoService.DeleteProductoAsync(id);
                if (result)
                {
                    return Ok(new { message = "Producto eliminado exitosamente." });
                }
                return NotFound(new { message = "Producto no encontrado." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Hubo un problema al eliminar el producto.", error = ex.Message });
            }
        }
    }
}
