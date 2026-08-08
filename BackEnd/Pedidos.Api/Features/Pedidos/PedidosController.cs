using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pedidos.Api.Dtos;
using Pedidos.Api.Features.Pedidos.interfaces;

namespace Pedidos.Api.Features.Pedidos
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidosController : ControllerBase
    {
        private readonly ILogger<PedidosController> _logger;
        private readonly IPedidosServices _pedidosServices;

        private readonly IValidator<PedidosCreateDto> _validator;

        public PedidosController(ILogger<PedidosController> logger, IPedidosServices pedidosServices, IValidator<PedidosCreateDto> validator)
        {
            _logger = logger;
            _pedidosServices = pedidosServices;
            _validator = validator;
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
        [ProducesResponseType(typeof(PedidosResponseDto), StatusCodes.Status201Created)]
       // [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CrearPedidos(PedidosCreateDto request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _pedidosServices.CrearPedidoAsync(request, cancellationToken);

            return CreatedAtAction(nameof(ConsultarPedido), new { id = result.Id }, result);
        }

    }
}