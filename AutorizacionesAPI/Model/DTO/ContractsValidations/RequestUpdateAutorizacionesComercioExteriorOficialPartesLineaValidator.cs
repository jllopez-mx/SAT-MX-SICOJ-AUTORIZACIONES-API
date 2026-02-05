using FluentValidation;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.DTO.ContractsValidations
{
    public class RequestUpdateAutorizacionesComercioExteriorOficialPartesLineaValidator : AbstractValidator<RequestComercioExteriorOficialPartesUpdate>
    {
        public RequestUpdateAutorizacionesComercioExteriorOficialPartesLineaValidator()
        {
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

            RuleFor(c => c.FechaRecepcion)
                .NotNull()
                .NotEmpty()
                .WithMessage("Fecha recepción es requerida.")
                .Must(FluentValidationGuard.BeValidateDateFormat)
                .WithMessage("El formato de la fecha de recepción no es válido");
        }
    }
}
