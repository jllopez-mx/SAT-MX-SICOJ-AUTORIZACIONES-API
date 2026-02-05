using AutorizacionesAPI.Model.ViewModels.Enums;
using FluentValidation;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.DTO.ContractsValidations
{
    public class RequestResolucionCreateValidator : AbstractValidator<RequestResolucionCreate>
    {
        public RequestResolucionCreateValidator() 
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

            RuleFor(c => c.oficioResolucion)
                .NotNull().WithMessage("Número Oficio es requerido")
                .NotEmpty().WithMessage("Número Oficio es requerido")
                .Length(1, 40).WithMessage("Número oficio debe contener entre 1 y 40 caracteres.");

            RuleFor(c => c.fechaOficio)
                .NotNull()
                .NotEmpty()
                .WithMessage("Fecha oficio es requerida.")
                .Must(FluentValidationGuard.BeValidateDateFormat)
                .WithMessage("El formato de la fecha oficio no es válido");

            RuleFor(c => c.idSentido)
                .NotNull()
                .WithMessage("Sentido es requerido.")
                .GreaterThan(0)
                .WithMessage("Sentido requiere un valor mayor a 0.");
        }
    }
}
