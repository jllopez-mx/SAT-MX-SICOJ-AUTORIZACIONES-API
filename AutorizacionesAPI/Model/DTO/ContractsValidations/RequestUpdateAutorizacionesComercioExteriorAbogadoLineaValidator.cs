using FluentValidation;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.DTO.ContractsValidations
{
    public class RequestUpdateAutorizacionesComercioExteriorAbogadoLineaValidator : AbstractValidator<RequestComercioExteriorAbogadoUpdate>
    {
        public RequestUpdateAutorizacionesComercioExteriorAbogadoLineaValidator()
        {
            RuleFor(c => c.Id)
                .NotNull()
                .WithMessage("El identificador es requerido.")
                .GreaterThan(0)
                .WithMessage("El identificador no es válido.");

            RuleFor(c => c.IdTipoAutorizacion)
                .NotNull()
                .WithMessage("Tipo autorización es requerido.")
                .GreaterThan(0)
                .WithMessage("Tipo autorización requiere un valor mayor a 0.");

            RuleFor(c => c.PromoventeNoContribuyente)
                .NotNull()
                .WithMessage("Debe de especificar si el promovente no es el contribuyente.");

            RuleFor(c => c.RfcContribuyente)
                .Empty()
                .WithMessage("El RFC del contribuyente no es requerido.")
                .When(c => !c.PromoventeNoContribuyente);

            RuleFor(c => c.RfcContribuyente)
                .NotNull()
                .When(c => c.PromoventeNoContribuyente)
                .NotEmpty()
                .When(c => c.PromoventeNoContribuyente)
                .WithMessage("RFC es obligatorio.")
                .Length(12, 13)
                .When(c => c.PromoventeNoContribuyente)
                .WithMessage("RFC requiere entre 12 y 13 caracteres.");

            RuleFor(c => c.Contribuyente)
                .Empty()
                .WithMessage("El contribuyente no es requerido.")
                .When(c => !c.PromoventeNoContribuyente);

            RuleFor(c => c.Contribuyente)
                .NotNull()
                .WithMessage("Contribuyente es requerido")
                .When(c => c.PromoventeNoContribuyente)
                .NotEmpty()
                .WithMessage("Contribuyente es requerido")
                .When(c => c.PromoventeNoContribuyente);

            RuleFor(c => c.FechaPresentacion)
                .NotNull()
                .WithMessage("Fecha presentación es requerida.")
                .NotEmpty()
                .WithMessage("Fecha presentación es requerida.")
                .Must(FluentValidationGuard.BeValidateDateFormat)
                .WithMessage("El formato de la fecha de presentación no es válido");

            RuleFor(c => c.FechaRecepcion)
                .NotNull()
                .NotEmpty()
                .WithMessage("Fecha recepción es requerida.")
                .Must(FluentValidationGuard.BeValidateDateFormat)
                .WithMessage("El formato de la fecha de recepción no es válido");
        }
    }
}