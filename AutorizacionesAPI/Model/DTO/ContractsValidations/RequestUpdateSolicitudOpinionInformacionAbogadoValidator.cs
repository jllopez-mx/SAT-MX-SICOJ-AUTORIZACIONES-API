using AutorizacionesAPI.Model.ViewModels.Enums;
using FluentValidation;
using Sicoj.Utils;

namespace AutorizacionesAPI.Model.DTO.ContractsValidations
{
    public class RequestUpdateSolicitudOpinionInformacionAbogadoValidator : AbstractValidator<RequestSolicitudOpinionInformacionAbogadoUpdate>
    {
        public RequestUpdateSolicitudOpinionInformacionAbogadoValidator()
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
                .WithMessage("Fecha Solicitud es requerida.")
                .Must(FluentValidationGuard.BeValidateDateFormat)
                .WithMessage("El formato de la fecha de oficio de solicitud no es válido");

            RuleFor(c => c.noOficioRespuesta)
               .Must(value => string.IsNullOrEmpty(value) || value == "undefined" || System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Za-z0-9/()-]*$"))
               .WithMessage("El número de oficio de respuesta contiene caracteres no permitidos. Solo se permiten letras, números, diagonal (/), guion medio (-) y paréntesis ().");

            RuleFor(x => x.fechaRecepcion)
                .Must(date => date == null || date == "" || FluentValidationGuard.BeValidateDateFormat(date))
                .WithMessage("El formato de la fecha de recepción no es válido.")
                .When(x => x.fechaRecepcion != "undefined");

            RuleFor(x => x.fechaOficioRespuesta)
               .Must(date => date == null || date == "" || FluentValidationGuard.BeValidateDateFormat(date))
               .WithMessage("El formato de la fecha de recepción no es válido.")
               .When(x => x.fechaOficioRespuesta != "undefined");

        }
    }
    public class RequestSolicitudUpdateOnlyFileValidatorAbogado : AbstractValidator<RequestSolicitudOpinionInformacionAbogadoUpdate>
    {
        public RequestSolicitudUpdateOnlyFileValidatorAbogado()
        {
            RuleFor(c => c.numeroFolio)
                .NotNull().WithMessage("Número folio es requerido.")
                .NotEmpty().WithMessage("Número folio es requerido.")
                .Length(12).WithMessage("Número folio debe de contener 12 caracteres.");

            RuleFor(c => c.idTipoArchivo)
                .NotNull()
                .WithMessage("Tipo Archivo es requerido.")
                .GreaterThan(0)
                .WithMessage("Tipo Archivo requiere un valor mayor a 0.");

            RuleFor(c => c.idSeccion)
                .NotNull()
                .WithMessage("Sección es requerido.")
                .GreaterThan(0)
                .WithMessage("Sección requiere un valor mayor a 0.");

        }
    }

}
