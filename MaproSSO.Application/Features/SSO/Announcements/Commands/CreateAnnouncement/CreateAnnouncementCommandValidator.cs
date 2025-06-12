using FluentValidation;

namespace MaproSSO.Application.Features.SSO.Announcements.Commands.CreateAnnouncement
{
    public class CreateAnnouncementCommandValidator : AbstractValidator<CreateAnnouncementCommand>
    {
        public CreateAnnouncementCommandValidator()
        {
            RuleFor(x => x.AreaId)
                .NotEmpty().WithMessage("El área es requerida");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es requerido")
                .MaximumLength(200).WithMessage("El título no puede exceder 200 caracteres");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es requerida")
                .MaximumLength(4000).WithMessage("La descripción no puede exceder 4000 caracteres");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Tipo de anuncio inválido");

            RuleFor(x => x.Severity)
                .IsInEnum().WithMessage("Severidad inválida");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("La ubicación es requerida")
                .MaximumLength(200).WithMessage("La ubicación no puede exceder 200 caracteres");

            RuleFor(x => x.Images)
                .Must(images => images == null || images.Count <= 5)
                .WithMessage("No se pueden adjuntar más de 5 imágenes");

            RuleForEach(x => x.Images).ChildRules(image =>
            {
                image.RuleFor(i => i.ImageUrl)
                    .NotEmpty().WithMessage("La URL de la imagen es requerida")
                    .Must(BeAValidUrl).WithMessage("La URL de la imagen no es válida");
            });
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}