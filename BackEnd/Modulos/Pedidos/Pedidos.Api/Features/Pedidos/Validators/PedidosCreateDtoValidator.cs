using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Pedidos.Api.Dtos;

namespace Pedidos.Api.Features.Pedidos
{
    public class PedidosCreateDtoValidator : AbstractValidator<PedidosCreateDto> 
    {
        public PedidosCreateDtoValidator()
    {
        RuleFor(x => x.ClienteNombre)
            .NotEmpty()
            .WithMessage("El nombre del cliente es obligatorio.")
            .MaximumLength(100)
            .WithMessage("El nombre del cliente no puede superar los 100 caracteres.");

        RuleFor(x => x.Sku)
            .NotEmpty()
            .WithMessage("El SKU es obligatorio.")
            .MaximumLength(50)
            .WithMessage("El SKU no puede superar los 50 caracteres.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor que 0.");
    }
    }
}