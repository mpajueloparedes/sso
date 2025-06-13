//using MaproSSO.Domain.Common;
//using MaproSSO.Domain.Exceptions;
//using System.Reflection.Metadata;

//namespace MaproSSO.Domain.Entities.SSO
//{
//    public class DocumentFolder : BaseAuditableEntity
//    {
//        private readonly List<Document> _documents = new();

//        public Guid TenantId { get; private set; }
//        public Guid PillarId { get; private set; }
//        public Guid AreaId { get; private set; }
//        public string FolderName { get; private set; }
//        public Guid? ParentFolderId { get; private set; }
//        public string Path { get; private set; }
//        public bool IsSystemFolder { get; private set; }

//        public IReadOnlyCollection<Document> Documents => _documents.AsReadOnly();

//        private DocumentFolder() { }

//        public static DocumentFolder Create(
//            Guid tenantId,
//            Guid pillarId,
//            Guid areaId,
//            string folderName,
//            string path,
//            Guid? parentFolderId = null,
//            bool isSystemFolder = false,
//            Guid? createdBy = null)
//        {
//            if (string.IsNullOrWhiteSpace(folderName))
//                throw new DomainException("El nombre de la carpeta es requerido");

//            if (string.IsNullOrWhiteSpace(path))
//                throw new DomainException("La ruta de la carpeta es requerida");

//            var folder = new DocumentFolder
//            {
//                TenantId = tenantId,
//                PillarId = pillarId,
//                AreaId = areaId,
//                FolderName = folderName,
//                ParentFolderId = parentFolderId,
//                Path = path,
//                IsSystemFolder = isSystemFolder
//            };

//            if (createdBy.HasValue)
//                folder.SetCreatedInfo(createdBy.Value);

//            return folder;
//        }

//        public void Rename(string newName, Guid updatedBy)
//        {
//            if (IsSystemFolder)
//                throw new BusinessRuleValidationException("Las carpetas del sistema no pueden ser renombradas");

//            FolderName = newName;
//            SetUpdatedInfo(updatedBy);
//        }

//        public void UpdatePath(string newPath)
//        {
//            Path = newPath;
//        }
//    }
//}