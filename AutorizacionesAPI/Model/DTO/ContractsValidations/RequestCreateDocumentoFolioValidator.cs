using AutorizacionesAPI.Model.ViewModels.Enums;
using FluentValidation;
using Sicoj.Utils;
using Sicoj.Utils.Files;

namespace AutorizacionesAPI.Model.DTO.ContractsValidations
{
    public class RequestCreateDocumentoFolioValidator : AbstractValidator<RequestDocumentoFolioCreate>
    {
        public RequestCreateDocumentoFolioValidator() {
            RuleFor(c => c.documento)
                .NotNull().WithMessage("Documento es requerido.")
                .Must(c => FluentValidationGuard.ConvertBytesToMegaBytes(c.Length) <= 10).WithMessage("El tamaño del documento no puede ser mayor a 10MB.");

            RuleFor(c => c.idTipoAsunto)
                .NotNull()
                .WithMessage("Tipo asunto es requerido")
                .Must(c => Enum.IsDefined(typeof(EnumTipoAsunto), c))
                .WithMessage("Tipo asunto no es válido.");

            RuleFor(c => c.idTipoArchivo)
                .GreaterThan(0).WithMessage("Tipo asunto no es válido.");

            RuleFor(c => c.idAsunto)
                .GreaterThan(0).WithMessage("El identificador de la autorización no es válido.");

            RuleFor(c => c.idSeccion)
                .GreaterThan(0).WithMessage("Sección no es válida.");

            RuleFor(c => c.numeroFolio)
                .NotNull().WithMessage("Número folio es requerido.")
                .NotEmpty().WithMessage("Número folio es requerido.")
                .Length(12).WithMessage("Número folio debe de contener 12 caracteres.");
        }
    }
}
