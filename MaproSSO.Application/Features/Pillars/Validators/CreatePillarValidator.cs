using FluentValidation;
using MaproSSO.Application.Features.Pillars.Commands;
using MaproSSO.Application.Features.Pillars.Queries;

namespace MaproSSO.Application.Features.Pillars.Validators;

public class CreatePillarValidator : AbstractValidator<CreatePillarCommand>
{
    public CreatePillarValidator()
    {
        RuleFor(x => x.PillarName)
            .NotEmpty().WithMessage("Pillar name is required")
            .MaximumLength(200).WithMessage("Pillar name cannot exceed 200 characters");

        RuleFor(x => x.PillarCode)
            .NotEmpty().WithMessage("Pillar code is required")
            .MaximumLength(50).WithMessage("Pillar code cannot exceed 50 characters")
            .Matches("^[A-Z0-9_]+$").WithMessage("Pillar code can only contain uppercase letters, numbers, and underscores");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("Icon cannot exceed 100 characters");

        RuleFor(x => x.Color)
            .MaximumLength(10).WithMessage("Color cannot exceed 10 characters")
            .Matches("^#[0-9A-Fa-f]{6}$").When(x => !string.IsNullOrEmpty(x.Color))
            .WithMessage("Color must be a valid hex color code (e.g., #FF0000)");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Sort order must be greater than or equal to 0");
    }
}

public class UpdatePillarValidator : AbstractValidator<UpdatePillarCommand>
{
    public UpdatePillarValidator()
    {
        RuleFor(x => x.PillarId)
            .NotEmpty().WithMessage("Pillar ID is required");

        RuleFor(x => x.PillarName)
            .NotEmpty().WithMessage("Pillar name is required")
            .MaximumLength(200).WithMessage("Pillar name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Icon)
            .MaximumLength(100).WithMessage("Icon cannot exceed 100 characters");

        RuleFor(x => x.Color)
            .MaximumLength(10).WithMessage("Color cannot exceed 10 characters")
            .Matches("^#[0-9A-Fa-f]{6}$").When(x => !string.IsNullOrEmpty(x.Color))
            .WithMessage("Color must be a valid hex color code (e.g., #FF0000)");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Sort order must be greater than or equal to 0");
    }
}

public class CreateFolderValidator : AbstractValidator<CreateFolderCommand>
{
    public CreateFolderValidator()
    {
        RuleFor(x => x.PillarId)
            .NotEmpty().WithMessage("Pillar ID is required");

        RuleFor(x => x.AreaId)
            .NotEmpty().WithMessage("Area ID is required");

        RuleFor(x => x.FolderName)
            .NotEmpty().WithMessage("Folder name is required")
            .MaximumLength(200).WithMessage("Folder name cannot exceed 200 characters")
            .Must(NotContainInvalidCharacters)
            .WithMessage("Folder name contains invalid characters");
    }

    private bool NotContainInvalidCharacters(string folderName)
    {
        if (string.IsNullOrEmpty(folderName))
            return true;

        var invalidChars = new[] { '/', '\\', ':', '*', '?', '"', '<', '>', '|' };
        return !folderName.Any(c => invalidChars.Contains(c));
    }
}

public class UpdateFolderValidator : AbstractValidator<UpdateFolderCommand>
{
    public UpdateFolderValidator()
    {
        RuleFor(x => x.FolderId)
            .NotEmpty().WithMessage("Folder ID is required");

        RuleFor(x => x.FolderName)
            .NotEmpty().WithMessage("Folder name is required")
            .MaximumLength(200).WithMessage("Folder name cannot exceed 200 characters")
            .Must(NotContainInvalidCharacters)
            .WithMessage("Folder name contains invalid characters");
    }

    private bool NotContainInvalidCharacters(string folderName)
    {
        if (string.IsNullOrEmpty(folderName))
            return true;

        var invalidChars = new[] { '/', '\\', ':', '*', '?', '"', '<', '>', '|' };
        return !folderName.Any(c => invalidChars.Contains(c));
    }
}

public class UploadDocumentValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentValidator()
    {
        RuleFor(x => x.FolderId)
            .NotEmpty().WithMessage("Folder ID is required");

        RuleFor(x => x.FileName)
            .NotEmpty().WithMessage("File name is required")
            .MaximumLength(500).WithMessage("File name cannot exceed 500 characters")
            .Must(NotContainInvalidCharacters)
            .WithMessage("File name contains invalid characters");

        RuleFor(x => x.ContentType)
            .NotEmpty().WithMessage("Content type is required")
            .MaximumLength(100).WithMessage("Content type cannot exceed 100 characters");

        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0).WithMessage("File size must be greater than 0")
            .LessThanOrEqualTo(100 * 1024 * 1024).WithMessage("File size cannot exceed 100MB");

        RuleFor(x => x.StorageUrl)
            .NotEmpty().WithMessage("Storage URL is required")
            .MaximumLength(1000).WithMessage("Storage URL cannot exceed 1000 characters");

        RuleFor(x => x.Tags)
            .MaximumLength(500).WithMessage("Tags cannot exceed 500 characters");
    }

    private bool NotContainInvalidCharacters(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return true;

        var invalidChars = new[] { '/', '\\', ':', '*', '?', '"', '<', '>', '|' };
        return !fileName.Any(c => invalidChars.Contains(c));
    }
}

public class UpdateDocumentValidator : AbstractValidator<UpdateDocumentCommand>
{
    public UpdateDocumentValidator()
    {
        RuleFor(x => x.DocumentId)
            .NotEmpty().WithMessage("Document ID is required");

        RuleFor(x => x.FileName)
            .MaximumLength(500).WithMessage("File name cannot exceed 500 characters")
            .Must(NotContainInvalidCharacters).When(x => !string.IsNullOrEmpty(x.FileName))
            .WithMessage("File name contains invalid characters");

        RuleFor(x => x.Tags)
            .MaximumLength(500).WithMessage("Tags cannot exceed 500 characters");
    }

    private bool NotContainInvalidCharacters(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return true;

        var invalidChars = new[] { '/', '\\', ':', '*', '?', '"', '<', '>', '|' };
        return !fileName.Any(c => invalidChars.Contains(c));
    }
}

public class SearchDocumentsValidator : AbstractValidator<SearchDocumentsQuery>
{
    public SearchDocumentsValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotEmpty().WithMessage("Search term is required")
            .MinimumLength(2).WithMessage("Search term must be at least 2 characters")
            .MaximumLength(100).WithMessage("Search term cannot exceed 100 characters");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate).When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("From date must be less than or equal to To date");
    }
}