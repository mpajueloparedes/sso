using MaproSSO.Domain.Common;
using MaproSSO.Domain.Exceptions;

namespace MaproSSO.Domain.Entities.SSO
{
    public class Document : BaseAuditableEntity
    {
        public Guid TenantId { get; private set; }
        public Guid FolderId { get; private set; }
        public string FileName { get; private set; }
        public string FileExtension { get; private set; }
        public long FileSizeBytes { get; private set; }
        public string ContentType { get; private set; }
        public string StorageUrl { get; private set; }
        public int Version { get; private set; }
        public bool IsCurrentVersion { get; private set; }
        public Guid? ParentDocumentId { get; private set; }
        public string Tags { get; private set; }

        private Document() { }

        public static Document Create(
            Guid tenantId,
            Guid folderId,
            string fileName,
            string fileExtension,
            long fileSizeBytes,
            string contentType,
            string storageUrl,
            string tags = null,
            Guid? createdBy = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new DomainException("El nombre del archivo es requerido");

            if (string.IsNullOrWhiteSpace(fileExtension))
                throw new DomainException("La extensión del archivo es requerida");

            if (fileSizeBytes <= 0)
                throw new DomainException("El tamaño del archivo debe ser mayor a cero");

            if (string.IsNullOrWhiteSpace(contentType))
                throw new DomainException("El tipo de contenido es requerido");

            if (string.IsNullOrWhiteSpace(storageUrl))
                throw new DomainException("La URL de almacenamiento es requerida");

            var document = new Document
            {
                TenantId = tenantId,
                FolderId = folderId,
                FileName = fileName,
                FileExtension = fileExtension.ToLowerInvariant(),
                FileSizeBytes = fileSizeBytes,
                ContentType = contentType,
                StorageUrl = storageUrl,
                Version = 1,
                IsCurrentVersion = true,
                Tags = tags
            };

            if (createdBy.HasValue)
                document.SetCreatedInfo(createdBy.Value);

            return document;
        }

        public Document CreateNewVersion(
            string fileName,
            long fileSizeBytes,
            string storageUrl,
            Guid createdBy)
        {
            // Marcar la versión actual como no actual
            IsCurrentVersion = false;

            // Crear nueva versión
            var newVersion = new Document
            {
                TenantId = TenantId,
                FolderId = FolderId,
                FileName = fileName,
                FileExtension = FileExtension,
                FileSizeBytes = fileSizeBytes,
                ContentType = ContentType,
                StorageUrl = storageUrl,
                Version = Version + 1,
                IsCurrentVersion = true,
                ParentDocumentId = ParentDocumentId ?? Id,
                Tags = Tags
            };

            newVersion.SetCreatedInfo(createdBy);

            return newVersion;
        }

        public void UpdateTags(string tags, Guid updatedBy)
        {
            Tags = tags;
            SetUpdatedInfo(updatedBy);
        }
    }
}