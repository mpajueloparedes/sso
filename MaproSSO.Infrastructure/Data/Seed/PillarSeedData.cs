using MaproSSO.Domain.Entities.Pillars;

namespace MaproSSO.Infrastructure.Data.Seed;

public static class PillarSeedData
{
    public static List<Pillar> GetDefaultPillars(Guid tenantId, Guid createdBy)
    {
        return new List<Pillar>
        {
            new Pillar
            {
                PillarId = Guid.NewGuid(),
                TenantId = tenantId,
                PillarName = "Liderazgo y Compromiso",
                PillarCode = "LEADERSHIP",
                Description = "Liderazgo visible y compromiso de la alta dirección en seguridad y salud ocupacional",
                Icon = "fas fa-crown",
                Color = "#FF6B35",
                SortOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            },
            new Pillar
            {
                PillarId = Guid.NewGuid(),
                TenantId = tenantId,
                PillarName = "Política de SSO",
                PillarCode = "POLICY",
                Description = "Política clara y definida de seguridad y salud ocupacional",
                Icon = "fas fa-file-contract",
                Color = "#4ECDC4",
                SortOrder = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            },
            new Pillar
            {
                PillarId = Guid.NewGuid(),
                TenantId = tenantId,
                PillarName = "Planificación",
                PillarCode = "PLANNING",
                Description = "Planificación estratégica y operativa para la gestión de SSO",
                Icon = "fas fa-calendar-alt",
                Color = "#45B7D1",
                SortOrder = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            },
            new Pillar
            {
                PillarId = Guid.NewGuid(),
                TenantId = tenantId,
                PillarName = "Apoyo y Recursos",
                PillarCode = "SUPPORT",
                Description = "Recursos humanos, técnicos y financieros para el sistema SSO",
                Icon = "fas fa-hands-helping",
                Color = "#96CEB4",
                SortOrder = 4,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            },
            new Pillar
            {
                PillarId = Guid.NewGuid(),
                TenantId = tenantId,
                PillarName = "Operación",
                PillarCode = "OPERATION",
                Description = "Implementación y ejecución de los procesos operacionales de SSO",
                Icon = "fas fa-cogs",
                Color = "#FFEAA7",
                SortOrder = 5,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            },
            new Pillar
            {
                PillarId = Guid.NewGuid(),
                TenantId = tenantId,
                PillarName = "Evaluación del Desempeño",
                PillarCode = "PERFORMANCE",
                Description = "Monitoreo, medición y evaluación del desempeño del sistema SSO",
                Icon = "fas fa-chart-line",
                Color = "#DDA0DD",
                SortOrder = 6,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            },
            new Pillar
            {
                PillarId = Guid.NewGuid(),
                TenantId = tenantId,
                PillarName = "Mejora Continua",
                PillarCode = "IMPROVEMENT",
                Description = "Procesos de mejora continua y corrección de no conformidades",
                Icon = "fas fa-arrow-up",
                Color = "#74B9FF",
                SortOrder = 7,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            },
            new Pillar
            {
                PillarId = Guid.NewGuid(),
                TenantId = tenantId,
                PillarName = "Gestión de Contratistas",
                PillarCode = "CONTRACTORS",
                Description = "Gestión y control de empresas contratistas y subcontratistas",
                Icon = "fas fa-handshake",
                Color = "#FD79A8",
                SortOrder = 8,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            },
            new Pillar
            {
                PillarId = Guid.NewGuid(),
                TenantId = tenantId,
                PillarName = "Preparación para Emergencias",
                PillarCode = "EMERGENCY",
                Description = "Planes de preparación y respuesta ante situaciones de emergencia",
                Icon = "fas fa-exclamation-triangle",
                Color = "#E17055",
                SortOrder = 9,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            }
        };
    }

    public static List<DocumentFolder> GetDefaultFolders(List<Pillar> pillars, Guid areaId, Guid createdBy)
    {
        var folders = new List<DocumentFolder>();

        foreach (var pillar in pillars)
        {
            // Create root folders for each pillar
            var rootFolders = new[]
            {
                new DocumentFolder
                {
                    FolderId = Guid.NewGuid(),
                    TenantId = pillar.TenantId,
                    PillarId = pillar.PillarId,
                    AreaId = areaId,
                    FolderName = "Procedimientos",
                    Path = "Procedimientos",
                    IsSystemFolder = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy
                },
                new DocumentFolder
                {
                    FolderId = Guid.NewGuid(),
                    TenantId = pillar.TenantId,
                    PillarId = pillar.PillarId,
                    AreaId = areaId,
                    FolderName = "Registros",
                    Path = "Registros",
                    IsSystemFolder = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy
                },
                new DocumentFolder
                {
                    FolderId = Guid.NewGuid(),
                    TenantId = pillar.TenantId,
                    PillarId = pillar.PillarId,
                    AreaId = areaId,
                    FolderName = "Formatos",
                    Path = "Formatos",
                    IsSystemFolder = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy
                }
            };

            folders.AddRange(rootFolders);
        }

        return folders;
    }
}