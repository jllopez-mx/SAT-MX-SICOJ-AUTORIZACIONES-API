using AutorizacionesAPI.Model.ViewModels.Enums;
using FluentValidation;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.DTO.ContractsValidations
{
    public class RequestCreateSolicitudOpinionInformacionValidator : AbstractValidator<RequestCreateSolicitudOpinionInformacion>
    {
        public RequestCreateSolicitudOpinionInformacionValidator()
        {
            RuleFor(c => c.idAsunto)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("El valor indicado para el id debe ser un valor mayor a cero.");

            RuleFor(c => c.idTipoAsunto)
                .NotNull()
                .WithMessage("Tipo asunto es requerido")
                .Must(c => Enum.IsDefined(typeof(EnumTipoAsunto), c))
                .WithMessage("Tipo asunto no es válido.");

            RuleFor(c => c.idUnidadAdministrativa)
               .NotEmpty()
               .GreaterThan(0)
               .WithMessage("El valor indicado para el id debe ser un valor mayor a cero.")
               .When(c => c.unidadInterna);

            RuleFor(c => c.noOficioSolicitud)
                .NotEmpty()
                .WithMessage("No ha indicado el parámetro de número de oficio de solicitud.")
                .Matches(@"^[A-Za-z0-9/()-]*$")
                .WithMessage("El número de oficio contiene caracteres no permitidos. Solo se permiten letras, números, diagonal (/), guion medio (-) y paréntesis ().")
                .Must(x => !x.Contains(" "))
                .WithMessage("El número de oficio de solicitud no debe contener espacios en blanco.");

            RuleFor(c => c.fechaOficioSolicitud)
                .NotNull()
                .NotEmpty()
                .WithMessage("Fecha Requerimiento es requerida.")
                .Must(FluentValidationGuard.BeValidateDateFormat)
                .WithMessage("El formato de la fecha de oficio de solicitud no es válido");
        }
    }
}
