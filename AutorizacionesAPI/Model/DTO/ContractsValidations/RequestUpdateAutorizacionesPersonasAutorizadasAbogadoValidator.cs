using FluentValidation;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.DTO.ContractsValidations
{
    public class RequestUpdateAutorizacionesPersonasAutorizadasAbogadoValidator : AbstractValidator<RequestPersonasAutorizadasAbogadoUpdate>
    {
        public RequestUpdateAutorizacionesPersonasAutorizadasAbogadoValidator()
        {
            RuleFor(c => c.nombre)
                .NotNull()
                .WithMessage("El nombre es requerido.")
                .NotEmpty()
                .WithMessage("El nombre es requerido.");

            RuleFor(c => c.rfc)
                .NotNull()
                .WithMessage("El RFC es requerido.")
                .NotEmpty()
                .WithMessage("El RFC es requerido.")
                .Length(12, 13)
                .WithMessage("RFC requiere entre 12 y 13 caracteres.");

            RuleFor(c => c.idAsunto)
               .NotNull()
               .WithMessage("Tipo autorización es requerido.")
               .GreaterThan(0)
               .WithMessage("Tipo autorización requiere un valor mayor a 0.");
        }
    }
}
