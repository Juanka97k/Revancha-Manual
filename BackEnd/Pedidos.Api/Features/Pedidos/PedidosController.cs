using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pedidos.Api.DTOS;

namespace Ordenes.Api.Features.Pedidos
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly ILogger<PedidosController> _logger;

        public PedidosController(ILogger<PedidosController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult ConsultarPedido()
        {
            return Ok("Soy el get");
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
        public IActionResult ConsultarPedido(int id)
        {
            return Ok($"Soy el get con id {id}");
        }

        [HttpPost]
        public IActionResult CrarPedidos(PedidosCreateDto request)
        {
            return CreatedAtAction(nameof(ConsultarPedido), new {id = request.Cantidad},request);
        }

    }
}