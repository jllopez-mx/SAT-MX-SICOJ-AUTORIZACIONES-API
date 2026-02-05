using AutorizacionesAPI.Model.ViewModels.Enums;
using FluentValidation;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.DTO.ContractsValidations
{
    public class RequestRequerimientoUpdateValidator : AbstractValidator<RequestRequerimientoUpdate>
    {
        public RequestRequerimientoUpdateValidator()
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
                .WithMessage("El identificador requiere un valor mayor a 0.");

            RuleFor(c => c.oficioRequerimiento)
                .NotNull().WithMessage("Número Oficio es requerido")
                .NotEmpty().WithMessage("Número Oficio es requerido")
                .Length(1, 40).WithMessage("Número oficio debe contener entre 1 y 40 caracteres.");

            RuleFor(c => c.fechaOficio)
                .NotNull()
                .NotEmpty()
                .WithMessage("Fecha oficio es requerida.")
                .Must(FluentValidationGuard.BeValidateDateFormat)
                .WithMessage("El formato de la fecha oficio no es válido");

            RuleFor(c => c.fechaNotificacion)
                .NotNull()
                .NotEmpty()
                .WithMessage("Fecha notificación en el SAT es requerida.")
                .Must(FluentValidationGuard.BeValidateDateFormat)
                .WithMessage("El formato de la fecha notificación en el SAT no es válido");

            RuleFor(c => c.fechaAtencion)
                .Empty()
                .WithMessage("Fecha atención no es requerido.")
                .When(c => c.atendioRequerimiento is null);

            RuleFor(c => c.fechaAtencion)
                .NotNull()
                .When(c => c.atendioRequerimiento is not null && c.atendioRequerimiento.GetValueOrDefault())
                .NotEmpty()
                .When(c => c.atendioRequerimiento is not null && c.atendioRequerimiento.GetValueOrDefault())
                .WithMessage("Fecha atención es obligatorio.")
                .Must(FluentValidationGuard.BeValidateDateFormat!)
                .When(c => c.atendioRequerimiento is not null && c.atendioRequerimiento.GetValueOrDefault())
                .WithMessage("El formato de la fecha atención no es válido");
        }
    }

    public class RequestRequerimientoUpdateOnlyFileValidator : AbstractValidator<RequestRequerimientoUpdate>
    {
        public RequestRequerimientoUpdateOnlyFileValidator()
        {
            RuleFor(c => c.idTipoArchivo)
                .NotNull()
                .WithMessage("Tipo Archivo es requerido.")
                .Must(c => c == EnumTipoDocumentoCons.OFICIO_DE_NOTIFICACIÓN || c == EnumTipoDocumentoCons.ESCRITO_DE_RESPUESTA)
                .WithMessage("Tipo Archivo no válido.");

            RuleFor(c => c.numeroFolio)
                .NotNull().WithMessage("Número folio es requerido.")
                .NotEmpty().WithMessage("Número folio es requerido.")
                .Length(12).WithMessage("Número folio debe de contener 12 caracteres.");
        }
    }
}