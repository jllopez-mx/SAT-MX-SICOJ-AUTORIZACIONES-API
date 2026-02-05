using AutorizacionesAPI.Model.ViewModels.Enums;
using FluentValidation;

namespace AutorizacionesAPI.Model.DTO.ContractsValidations
{
    public class RequestConcluirValidator : AbstractValidator<RequestConcluir>
    {
        public RequestConcluirValidator() {

            RuleFor(c => c.idTipoAsunto)
                .NotNull()
                .WithMessage("Tipo asunto es requerido")
                .Must(c => Enum.IsDefined(typeof(EnumTipoAsunto), c))
                .WithMessage("Tipo asunto no es válido.");
        }
    }
}
