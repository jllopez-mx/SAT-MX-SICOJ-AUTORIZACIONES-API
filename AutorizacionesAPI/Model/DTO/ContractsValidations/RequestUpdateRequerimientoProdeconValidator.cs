using AutorizacionesAPI.Model.ViewModels.Enums;
using FluentValidation;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.DTO.ContractsValidations
{
    public class RequestUpdateRequerimientoProdeconValidator : AbstractValidator<RequestRequerimientoProdeconUpdate>
    {
        public RequestUpdateRequerimientoProdeconValidator()
        {
            RuleFor(c => c.idAsunto)
                .NotNull()
                .WithMessage("El identificador de la Autorización es requerido.")
                .GreaterThan(0)
                .WithMessage("El identificador de la Autorización requiere un valor mayor a 0.");

            RuleFor(c => c.idTipoAsunto)
                .NotNull()
                .WithMessage("Tipo asunto es requerido")
                .Must(c => Enum.IsDefined(typeof(EnumTipoAsunto), c))
                .WithMessage("Tipo asunto no es válido.");

            RuleFor(c => c.id)
                .NotNull()
                .WithMessage("El identificador es requerido.")
                .GreaterThan(0)
                .WithMessage("El identificador no es válido.");

            RuleFor(c => c.numeroOficio)
                .NotNull().WithMessage("Número Oficio es requerido")
                .NotEmpty().WithMessage("Número Oficio es requerido")
                .Length(1, 40).WithMessage("Número oficio debe contener entre 1 y 40 caracteres.");

            RuleFor(c => c.numeroExpediente)
                .NotNull().WithMessage("Número Oficio es requerido")
                .NotEmpty().WithMessage("Número Oficio es requerido")
                .Length(1, 40).WithMessage("Número oficio debe contener debe contener entre 1 y 40 caracteres.");

            RuleFor(c => c.fechaOficio)
                .NotNull()
                .NotEmpty()
                .WithMessage("Fecha oficio es requerida.")
                .Must(FluentValidationGuard.BeValidateDateFormat)
                .WithMessage("El formato de la fecha oficio no es válido");

            RuleFor(c => c.fechaIngresoSat)
                .NotNull()
                .NotEmpty()
                .WithMessage("Fecha ingreso en el SAT es requerida.")
                .Must(FluentValidationGuard.BeValidateDateFormat)
                .WithMessage("El formato de la fecha ingreso en el SAT no es válido");

            RuleFor(c => c.requiereAccion)
                .NotNull()
                .WithMessage("Debe de especificar si requiere acción adicional.");

            RuleFor(c => c.atencion)
                .Empty()
                .WithMessage("Atención no es requerido.")
                .When(c => !c.requiereAccion);

            RuleFor(c => c.atencion)
                .NotNull()
                .When(c => c.requiereAccion)
                .NotEmpty()
                .When(c => c.requiereAccion)
                .WithMessage("Atención es obligatorio.")
                .MaximumLength(1000)
                .When(c => c.requiereAccion)
                .WithMessage("Atención debe de contener un máximo de 1000 caracteres.");
        }
    }
}
