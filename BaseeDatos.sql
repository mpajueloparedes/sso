-- =============================================
-- MaproSSO - Sistema de Gestión de Seguridad y Salud Ocupacional Minera
-- Script de Base de Datos - SQL Server 2022
-- Versión: 1.0.0
-- Fecha: 2025
-- =============================================

-- Crear base de datos
CREATE DATABASE GestionSSO
GO

USE GestionSSO
GO

-- =============================================
-- SCHEMAS
-- =============================================
CREATE SCHEMA [Tenant]
GO

CREATE SCHEMA [Security]
GO

CREATE SCHEMA [Subscription]
GO

CREATE SCHEMA [SSO]
GO

CREATE SCHEMA [Audit]
GO

-- =============================================
-- TABLAS DE TENANT (MULTITENANCY)
-- =============================================

-- Tabla principal de empresas/clientes
CREATE TABLE [Tenant].[Tenants] (
    [TenantId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [CompanyName] NVARCHAR(200) NOT NULL,
    [TradeName] NVARCHAR(200) NULL,
    [TaxId] NVARCHAR(20) NOT NULL,
    [Industry] NVARCHAR(100) NOT NULL,
    [EmployeeCount] INT NOT NULL DEFAULT 0,
    [Country] NVARCHAR(100) NOT NULL,
    [State] NVARCHAR(100) NOT NULL,
    [City] NVARCHAR(100) NOT NULL,
    [Address] NVARCHAR(500) NOT NULL,
    [PostalCode] NVARCHAR(20) NULL,
    [Phone] NVARCHAR(50) NOT NULL,
    [Email] NVARCHAR(200) NOT NULL,
    [Website] NVARCHAR(200) NULL,
    [LogoUrl] NVARCHAR(500) NULL,
    [TimeZone] NVARCHAR(100) NOT NULL DEFAULT 'America/Lima',
    [Culture] NVARCHAR(10) NOT NULL DEFAULT 'es-PE',
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(100) NULL,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(100) NULL,
    CONSTRAINT [PK_Tenants] PRIMARY KEY CLUSTERED ([TenantId]),
    CONSTRAINT [UQ_Tenants_TaxId] UNIQUE ([TaxId]),
    CONSTRAINT [UQ_Tenants_Email] UNIQUE ([Email])
)
GO

-- Configuración específica por tenant
CREATE TABLE [Tenant].[TenantSettings] (
    [SettingId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [SettingKey] NVARCHAR(100) NOT NULL,
    [SettingValue] NVARCHAR(MAX) NOT NULL,
    [DataType] NVARCHAR(50) NOT NULL, -- String, Int, Bool, Json
    [Description] NVARCHAR(500) NULL,
    [IsEncrypted] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [PK_TenantSettings] PRIMARY KEY CLUSTERED ([SettingId]),
    CONSTRAINT [FK_TenantSettings_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [UQ_TenantSettings_Key] UNIQUE ([TenantId], [SettingKey])
)
GO

-- =============================================
-- TABLAS DE SUSCRIPCIÓN
-- =============================================

-- Planes de suscripción
CREATE TABLE [Subscription].[Plans] (
    [PlanId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlanName] NVARCHAR(100) NOT NULL,
    [PlanType] NVARCHAR(50) NOT NULL, -- Basic, Standard, Premium
    [Description] NVARCHAR(500) NULL,
    [MonthlyPrice] DECIMAL(10,2) NOT NULL,
    [AnnualPrice] DECIMAL(10,2) NOT NULL,
    [Currency] NVARCHAR(3) NOT NULL DEFAULT 'USD',
    [TrialDays] INT NOT NULL DEFAULT 30,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [SortOrder] INT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [PK_Plans] PRIMARY KEY CLUSTERED ([PlanId]),
    CONSTRAINT [UQ_Plans_PlanType] UNIQUE ([PlanType]),
    CONSTRAINT [CK_Plans_PlanType] CHECK ([PlanType] IN ('Basic', 'Standard', 'Premium'))
)
GO

-- Features por plan
CREATE TABLE [Subscription].[PlanFeatures] (
    [FeatureId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PlanId] UNIQUEIDENTIFIER NOT NULL,
    [FeatureName] NVARCHAR(100) NOT NULL,
    [FeatureCode] NVARCHAR(50) NOT NULL,
    [FeatureType] NVARCHAR(50) NOT NULL, -- Module, Limit, Feature
    [Value] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [SortOrder] INT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_PlanFeatures] PRIMARY KEY CLUSTERED ([FeatureId]),
    CONSTRAINT [FK_PlanFeatures_Plan] FOREIGN KEY ([PlanId]) REFERENCES [Subscription].[Plans]([PlanId]),
    CONSTRAINT [UQ_PlanFeatures_Code] UNIQUE ([PlanId], [FeatureCode])
)
GO

-- Suscripciones de tenants
CREATE TABLE [Subscription].[Subscriptions] (
    [SubscriptionId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [PlanId] UNIQUEIDENTIFIER NOT NULL,
    [BillingCycle] NVARCHAR(20) NOT NULL, -- Monthly, Annual
    [Status] NVARCHAR(20) NOT NULL, -- Trial, Active, Suspended, Cancelled, Expired
    [StartDate] DATETIME2 NOT NULL,
    [EndDate] DATETIME2 NOT NULL,
    [TrialEndDate] DATETIME2 NULL,
    [NextBillingDate] DATETIME2 NULL,
    [CancellationDate] DATETIME2 NULL,
    [GracePeriodEndDate] DATETIME2 NULL,
    [AutoRenew] BIT NOT NULL DEFAULT 1,
    [PaymentMethodId] UNIQUEIDENTIFIER NULL,
    [CurrentPeriodStart] DATETIME2 NOT NULL,
    [CurrentPeriodEnd] DATETIME2 NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [PK_Subscriptions] PRIMARY KEY CLUSTERED ([SubscriptionId]),
    CONSTRAINT [FK_Subscriptions_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [FK_Subscriptions_Plan] FOREIGN KEY ([PlanId]) REFERENCES [Subscription].[Plans]([PlanId]),
    CONSTRAINT [CK_Subscriptions_Status] CHECK ([Status] IN ('Trial', 'Active', 'Suspended', 'Cancelled', 'Expired')),
    CONSTRAINT [CK_Subscriptions_BillingCycle] CHECK ([BillingCycle] IN ('Monthly', 'Annual'))
)
GO

-- Historial de cambios de suscripción
CREATE TABLE [Subscription].[SubscriptionHistory] (
    [HistoryId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [SubscriptionId] UNIQUEIDENTIFIER NOT NULL,
    [Action] NVARCHAR(50) NOT NULL, -- Created, Upgraded, Downgraded, Renewed, Suspended, Cancelled, Reactivated
    [FromPlanId] UNIQUEIDENTIFIER NULL,
    [ToPlanId] UNIQUEIDENTIFIER NULL,
    [FromBillingCycle] NVARCHAR(20) NULL,
    [ToBillingCycle] NVARCHAR(20) NULL,
    [Reason] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_SubscriptionHistory] PRIMARY KEY CLUSTERED ([HistoryId]),
    CONSTRAINT [FK_SubscriptionHistory_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [Subscription].[Subscriptions]([SubscriptionId])
)
GO

-- Pagos
CREATE TABLE [Subscription].[Payments] (
    [PaymentId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [SubscriptionId] UNIQUEIDENTIFIER NOT NULL,
    [Amount] DECIMAL(10,2) NOT NULL,
    [Currency] NVARCHAR(3) NOT NULL DEFAULT 'USD',
    [PaymentDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [PaymentMethod] NVARCHAR(50) NOT NULL,
    [TransactionId] NVARCHAR(200) NULL,
    [Status] NVARCHAR(20) NOT NULL, -- Pending, Completed, Failed, Refunded
    [FailureReason] NVARCHAR(500) NULL,
    [InvoiceNumber] NVARCHAR(50) NOT NULL,
    [InvoiceUrl] NVARCHAR(500) NULL,
    [RefundedAmount] DECIMAL(10,2) NULL,
    [RefundedAt] DATETIME2 NULL,
    [Notes] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY CLUSTERED ([PaymentId]),
    CONSTRAINT [FK_Payments_Subscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [Subscription].[Subscriptions]([SubscriptionId]),
    CONSTRAINT [CK_Payments_Status] CHECK ([Status] IN ('Pending', 'Completed', 'Failed', 'Refunded'))
)
GO

-- Uso de features/límites
CREATE TABLE [Subscription].[FeatureUsage] (
    [UsageId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [FeatureCode] NVARCHAR(50) NOT NULL,
    [CurrentUsage] INT NOT NULL DEFAULT 0,
    [UsageLimit] INT NULL,
    [ResetPeriod] NVARCHAR(20) NULL, -- Daily, Monthly, Annual, Never
    [LastResetDate] DATETIME2 NULL,
    [UpdatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_FeatureUsage] PRIMARY KEY CLUSTERED ([UsageId]),
    CONSTRAINT [FK_FeatureUsage_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [UQ_FeatureUsage_Feature] UNIQUE ([TenantId], [FeatureCode])
)
GO

-- =============================================
-- TABLAS DE SEGURIDAD
-- =============================================

-- Usuarios del sistema
CREATE TABLE [Security].[Users] (
    [UserId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [Username] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(200) NOT NULL,
    [NormalizedEmail] NVARCHAR(200) NOT NULL,
    [EmailConfirmed] BIT NOT NULL DEFAULT 0,
    [PasswordHash] NVARCHAR(500) NOT NULL,
    [SecurityStamp] NVARCHAR(100) NOT NULL,
    [PhoneNumber] NVARCHAR(50) NULL,
    [PhoneNumberConfirmed] BIT NOT NULL DEFAULT 0,
    [TwoFactorEnabled] BIT NOT NULL DEFAULT 0,
    [TwoFactorMethod] NVARCHAR(20) NULL, -- SMS, Email, Authenticator
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [EmployeeCode] NVARCHAR(50) NULL,
    [Position] NVARCHAR(100) NULL,
    [Department] NVARCHAR(100) NULL,
    [PhotoUrl] NVARCHAR(500) NULL,
    [LockoutEnd] DATETIME2 NULL,
    [LockoutEnabled] BIT NOT NULL DEFAULT 1,
    [AccessFailedCount] INT NOT NULL DEFAULT 0,
    [PasswordChangedAt] DATETIME2 NULL,
    [LastLoginAt] DATETIME2 NULL,
    [LastLoginIP] NVARCHAR(50) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [MustChangePassword] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] NVARCHAR(100) NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] NVARCHAR(100) NULL,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] NVARCHAR(100) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId]),
    CONSTRAINT [FK_Users_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [UQ_Users_Username] UNIQUE ([TenantId], [Username]),
    CONSTRAINT [UQ_Users_Email] UNIQUE ([Email])
)
GO

-- Roles del sistema
CREATE TABLE [Security].[Roles] (
    [RoleId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [RoleName] NVARCHAR(100) NOT NULL,
    [NormalizedRoleName] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [IsSystemRole] BIT NOT NULL DEFAULT 0,
    [TenantId] UNIQUEIDENTIFIER NULL, -- NULL para roles del sistema
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2 NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleId]),
    CONSTRAINT [FK_Roles_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [UQ_Roles_Name] UNIQUE ([TenantId], [NormalizedRoleName])
)
GO

-- Relación usuarios-roles
CREATE TABLE [Security].[UserRoles] (
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    [AssignedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [AssignedBy] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserId], [RoleId]),
    CONSTRAINT [FK_UserRoles_User] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users]([UserId]),
    CONSTRAINT [FK_UserRoles_Role] FOREIGN KEY ([RoleId]) REFERENCES [Security].[Roles]([RoleId])
)
GO

-- Permisos del sistema
CREATE TABLE [Security].[Permissions] (
    [PermissionId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PermissionName] NVARCHAR(100) NOT NULL,
    [PermissionCode] NVARCHAR(100) NOT NULL,
    [Module] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED ([PermissionId]),
    CONSTRAINT [UQ_Permissions_Code] UNIQUE ([PermissionCode])
)
GO

-- Relación roles-permisos
CREATE TABLE [Security].[RolePermissions] (
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    [PermissionId] UNIQUEIDENTIFIER NOT NULL,
    [GrantedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [GrantedBy] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_RolePermissions] PRIMARY KEY CLUSTERED ([RoleId], [PermissionId]),
    CONSTRAINT [FK_RolePermissions_Role] FOREIGN KEY ([RoleId]) REFERENCES [Security].[Roles]([RoleId]),
    CONSTRAINT [FK_RolePermissions_Permission] FOREIGN KEY ([PermissionId]) REFERENCES [Security].[Permissions]([PermissionId])
)
GO

-- Tokens de actualización
CREATE TABLE [Security].[RefreshTokens] (
    [TokenId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Token] NVARCHAR(500) NOT NULL,
    [DeviceInfo] NVARCHAR(500) NULL,
    [IpAddress] NVARCHAR(50) NULL,
    [ExpiresAt] DATETIME2 NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [RevokedAt] DATETIME2 NULL,
    [RevokedBy] NVARCHAR(100) NULL,
    [RevokedReason] NVARCHAR(500) NULL,
    [ReplacedByToken] NVARCHAR(500) NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY CLUSTERED ([TokenId]),
    CONSTRAINT [FK_RefreshTokens_User] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users]([UserId]),
    CONSTRAINT [UQ_RefreshTokens_Token] UNIQUE ([Token])
)
GO

-- Historial de contraseñas
CREATE TABLE [Security].[PasswordHistory] (
    [HistoryId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [PasswordHash] NVARCHAR(500) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_PasswordHistory] PRIMARY KEY CLUSTERED ([HistoryId]),
    CONSTRAINT [FK_PasswordHistory_User] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users]([UserId])
)
GO

-- Configuración 2FA
CREATE TABLE [Security].[TwoFactorAuth] (
    [TwoFactorId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [SecretKey] NVARCHAR(500) NOT NULL,
    [QrCodeUrl] NVARCHAR(MAX) NULL,
    [BackupCodes] NVARCHAR(MAX) NULL, -- JSON array encriptado
    [EnabledAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [LastUsedAt] DATETIME2 NULL,
    CONSTRAINT [PK_TwoFactorAuth] PRIMARY KEY CLUSTERED ([TwoFactorId]),
    CONSTRAINT [FK_TwoFactorAuth_User] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users]([UserId]),
    CONSTRAINT [UQ_TwoFactorAuth_User] UNIQUE ([UserId])
)
GO

-- Sesiones activas
CREATE TABLE [Security].[UserSessions] (
    [SessionId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [SessionToken] NVARCHAR(500) NOT NULL,
    [DeviceType] NVARCHAR(50) NULL,
    [DeviceName] NVARCHAR(200) NULL,
    [Browser] NVARCHAR(100) NULL,
    [OperatingSystem] NVARCHAR(100) NULL,
    [IpAddress] NVARCHAR(50) NULL,
    [Location] NVARCHAR(200) NULL,
    [StartedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [LastActivityAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ExpiresAt] DATETIME2 NOT NULL,
    [EndedAt] DATETIME2 NULL,
    CONSTRAINT [PK_UserSessions] PRIMARY KEY CLUSTERED ([SessionId]),
    CONSTRAINT [FK_UserSessions_User] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users]([UserId]),
    CONSTRAINT [UQ_UserSessions_Token] UNIQUE ([SessionToken])
)
GO

-- =============================================
-- TABLAS DEL MÓDULO SSO
-- =============================================

-- Áreas de trabajo
CREATE TABLE [SSO].[Areas] (
    [AreaId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [AreaName] NVARCHAR(200) NOT NULL,
    [AreaCode] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [ParentAreaId] UNIQUEIDENTIFIER NULL,
    [ManagerUserId] UNIQUEIDENTIFIER NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Areas] PRIMARY KEY CLUSTERED ([AreaId]),
    CONSTRAINT [FK_Areas_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [FK_Areas_Parent] FOREIGN KEY ([ParentAreaId]) REFERENCES [SSO].[Areas]([AreaId]),
    CONSTRAINT [FK_Areas_Manager] FOREIGN KEY ([ManagerUserId]) REFERENCES [Security].[Users]([UserId]),
    CONSTRAINT [UQ_Areas_Code] UNIQUE ([TenantId], [AreaCode])
)
GO

-- Empresas contratistas
CREATE TABLE [SSO].[ContractorCompanies] (
    [ContractorId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [CompanyName] NVARCHAR(200) NOT NULL,
    [TaxId] NVARCHAR(20) NOT NULL,
    [ContactName] NVARCHAR(200) NOT NULL,
    [ContactEmail] NVARCHAR(200) NOT NULL,
    [ContactPhone] NVARCHAR(50) NOT NULL,
    [Address] NVARCHAR(500) NULL,
    [ContractStartDate] DATE NOT NULL,
    [ContractEndDate] DATE NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ContractorCompanies] PRIMARY KEY CLUSTERED ([ContractorId]),
    CONSTRAINT [FK_ContractorCompanies_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [UQ_ContractorCompanies_TaxId] UNIQUE ([TenantId], [TaxId])
)
GO

-- Usuarios de área
CREATE TABLE [SSO].[AreaUsers] (
    [AreaUserId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [AreaId] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [Role] NVARCHAR(50) NOT NULL, -- Leader, User
    [AssignedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [AssignedBy] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_AreaUsers] PRIMARY KEY CLUSTERED ([AreaUserId]),
    CONSTRAINT [FK_AreaUsers_Area] FOREIGN KEY ([AreaId]) REFERENCES [SSO].[Areas]([AreaId]),
    CONSTRAINT [FK_AreaUsers_User] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users]([UserId]),
    CONSTRAINT [UQ_AreaUsers_User] UNIQUE ([AreaId], [UserId])
)
GO

-- =============================================
-- MÓDULO 9 PILARES
-- =============================================

-- Pilares principales
CREATE TABLE [SSO].[Pillars] (
    [PillarId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [PillarName] NVARCHAR(200) NOT NULL,
    [PillarCode] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Icon] NVARCHAR(100) NULL,
    [Color] NVARCHAR(10) NULL,
    [SortOrder] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Pillars] PRIMARY KEY CLUSTERED ([PillarId]),
    CONSTRAINT [FK_Pillars_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [UQ_Pillars_Code] UNIQUE ([TenantId], [PillarCode])
)
GO

-- Carpetas de documentos
CREATE TABLE [SSO].[DocumentFolders] (
    [FolderId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [PillarId] UNIQUEIDENTIFIER NOT NULL,
    [AreaId] UNIQUEIDENTIFIER NOT NULL,
    [FolderName] NVARCHAR(200) NOT NULL,
    [ParentFolderId] UNIQUEIDENTIFIER NULL,
    [Path] NVARCHAR(1000) NOT NULL,
    [IsSystemFolder] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_DocumentFolders] PRIMARY KEY CLUSTERED ([FolderId]),
    CONSTRAINT [FK_DocumentFolders_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [FK_DocumentFolders_Pillar] FOREIGN KEY ([PillarId]) REFERENCES [SSO].[Pillars]([PillarId]),
    CONSTRAINT [FK_DocumentFolders_Area] FOREIGN KEY ([AreaId]) REFERENCES [SSO].[Areas]([AreaId]),
    CONSTRAINT [FK_DocumentFolders_Parent] FOREIGN KEY ([ParentFolderId]) REFERENCES [SSO].[DocumentFolders]([FolderId])
)
GO

-- Documentos
CREATE TABLE [SSO].[Documents] (
    [DocumentId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [FolderId] UNIQUEIDENTIFIER NOT NULL,
    [FileName] NVARCHAR(500) NOT NULL,
    [FileExtension] NVARCHAR(10) NOT NULL,
    [FileSizeBytes] BIGINT NOT NULL,
    [ContentType] NVARCHAR(100) NOT NULL,
    [StorageUrl] NVARCHAR(1000) NOT NULL,
    [Version] INT NOT NULL DEFAULT 1,
    [IsCurrentVersion] BIT NOT NULL DEFAULT 1,
    [ParentDocumentId] UNIQUEIDENTIFIER NULL,
    [Tags] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    [DeletedAt] DATETIME2 NULL,
    [DeletedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Documents] PRIMARY KEY CLUSTERED ([DocumentId]),
    CONSTRAINT [FK_Documents_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [FK_Documents_Folder] FOREIGN KEY ([FolderId]) REFERENCES [SSO].[DocumentFolders]([FolderId]),
    CONSTRAINT [FK_Documents_Parent] FOREIGN KEY ([ParentDocumentId]) REFERENCES [SSO].[Documents]([DocumentId])
)
GO

-- =============================================
-- MÓDULO ANUNCIOS
-- =============================================

-- Anuncios/Observaciones
CREATE TABLE [SSO].[Announcements] (
    [AnnouncementId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [AreaId] UNIQUEIDENTIFIER NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [Type] NVARCHAR(50) NOT NULL, -- Safety, Quality, Environment, Health
    [Severity] NVARCHAR(20) NOT NULL, -- Low, Medium, High, Critical
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, InProgress, Completed
    [Location] NVARCHAR(200) NOT NULL,
    [ReportedAt] DATETIME2 NOT NULL,
    [CompletedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Announcements] PRIMARY KEY CLUSTERED ([AnnouncementId]),
    CONSTRAINT [FK_Announcements_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [FK_Announcements_Area] FOREIGN KEY ([AreaId]) REFERENCES [SSO].[Areas]([AreaId]),
    CONSTRAINT [CK_Announcements_Status] CHECK ([Status] IN ('Pending', 'InProgress', 'Completed')),
    CONSTRAINT [CK_Announcements_Severity] CHECK ([Severity] IN ('Low', 'Medium', 'High', 'Critical'))
)
GO

-- Imágenes de anuncios
CREATE TABLE [SSO].[AnnouncementImages] (
    [ImageId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [AnnouncementId] UNIQUEIDENTIFIER NOT NULL,
    [ImageUrl] NVARCHAR(1000) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [SortOrder] INT NOT NULL DEFAULT 0,
    [UploadedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UploadedBy] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_AnnouncementImages] PRIMARY KEY CLUSTERED ([ImageId]),
    CONSTRAINT [FK_AnnouncementImages_Announcement] FOREIGN KEY ([AnnouncementId]) REFERENCES [SSO].[Announcements]([AnnouncementId])
)
GO

-- Acciones correctivas
CREATE TABLE [SSO].[CorrectiveActions] (
    [ActionId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [AnnouncementId] UNIQUEIDENTIFIER NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [ResponsibleUserId] UNIQUEIDENTIFIER NOT NULL,
    [DueDate] DATE NOT NULL,
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, InProgress, Completed
    [CompletedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_CorrectiveActions] PRIMARY KEY CLUSTERED ([ActionId]),
    CONSTRAINT [FK_CorrectiveActions_Announcement] FOREIGN KEY ([AnnouncementId]) REFERENCES [SSO].[Announcements]([AnnouncementId]),
    CONSTRAINT [FK_CorrectiveActions_Responsible] FOREIGN KEY ([ResponsibleUserId]) REFERENCES [Security].[Users]([UserId]),
    CONSTRAINT [CK_CorrectiveActions_Status] CHECK ([Status] IN ('Pending', 'InProgress', 'Completed'))
)
GO

-- Evidencias de acciones correctivas
CREATE TABLE [SSO].[ActionEvidences] (
    [EvidenceId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ActionId] UNIQUEIDENTIFIER NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [EvidenceUrl] NVARCHAR(1000) NULL,
    [UploadedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UploadedBy] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ActionEvidences] PRIMARY KEY CLUSTERED ([EvidenceId]),
    CONSTRAINT [FK_ActionEvidences_Action] FOREIGN KEY ([ActionId]) REFERENCES [SSO].[CorrectiveActions]([ActionId])
)
GO

-- =============================================
-- MÓDULO INSPECCIONES
-- =============================================

-- Programas de inspección
CREATE TABLE [SSO].[InspectionPrograms] (
    [ProgramId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [ProgramName] NVARCHAR(200) NOT NULL,
    [ProgramType] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Year] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_InspectionPrograms] PRIMARY KEY CLUSTERED ([ProgramId]),
    CONSTRAINT [FK_InspectionPrograms_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [UQ_InspectionPrograms_Name] UNIQUE ([TenantId], [ProgramName], [Year])
)
GO

-- Detalle de programas por área
CREATE TABLE [SSO].[InspectionProgramDetails] (
    [DetailId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ProgramId] UNIQUEIDENTIFIER NOT NULL,
    [AreaId] UNIQUEIDENTIFIER NOT NULL,
    [Frequency] NVARCHAR(20) NOT NULL, -- Daily, Weekly, Monthly, Quarterly, Annual
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_InspectionProgramDetails] PRIMARY KEY CLUSTERED ([DetailId]),
    CONSTRAINT [FK_InspectionProgramDetails_Program] FOREIGN KEY ([ProgramId]) REFERENCES [SSO].[InspectionPrograms]([ProgramId]),
    CONSTRAINT [FK_InspectionProgramDetails_Area] FOREIGN KEY ([AreaId]) REFERENCES [SSO].[Areas]([AreaId]),
    CONSTRAINT [CK_InspectionProgramDetails_Frequency] CHECK ([Frequency] IN ('Daily', 'Weekly', 'Monthly', 'Quarterly', 'Annual'))
)
GO

-- Inspecciones programadas
CREATE TABLE [SSO].[Inspections] (
    [InspectionId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [ProgramId] UNIQUEIDENTIFIER NOT NULL,
    [AreaId] UNIQUEIDENTIFIER NOT NULL,
    [InspectionCode] NVARCHAR(50) NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [InspectorUserId] UNIQUEIDENTIFIER NOT NULL,
    [ScheduledDate] DATE NOT NULL,
    [ExecutedDate] DATETIME2 NULL,
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, InProgress, Completed
    [CompletionPercentage] INT NOT NULL DEFAULT 0,
    [DocumentUrl] NVARCHAR(1000) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Inspections] PRIMARY KEY CLUSTERED ([InspectionId]),
    CONSTRAINT [FK_Inspections_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [FK_Inspections_Program] FOREIGN KEY ([ProgramId]) REFERENCES [SSO].[InspectionPrograms]([ProgramId]),
    CONSTRAINT [FK_Inspections_Area] FOREIGN KEY ([AreaId]) REFERENCES [SSO].[Areas]([AreaId]),
    CONSTRAINT [FK_Inspections_Inspector] FOREIGN KEY ([InspectorUserId]) REFERENCES [Security].[Users]([UserId]),
    CONSTRAINT [UQ_Inspections_Code] UNIQUE ([TenantId], [InspectionCode]),
    CONSTRAINT [CK_Inspections_Status] CHECK ([Status] IN ('Pending', 'InProgress', 'Completed')),
    CONSTRAINT [CK_Inspections_Percentage] CHECK ([CompletionPercentage] >= 0 AND [CompletionPercentage] <= 100)
)
GO

-- Observaciones de inspección
CREATE TABLE [SSO].[InspectionObservations] (
    [ObservationId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [InspectionId] UNIQUEIDENTIFIER NOT NULL,
    [ObservationCode] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [Type] NVARCHAR(50) NOT NULL, -- Safety, Quality, Environment, Compliance
    [Severity] NVARCHAR(20) NOT NULL, -- Low, Medium, High, Critical
    [ResponsibleUserId] UNIQUEIDENTIFIER NOT NULL,
    [DueDate] DATE NOT NULL,
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Pending', -- Pending, InProgress, Completed
    [CompletedAt] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_InspectionObservations] PRIMARY KEY CLUSTERED ([ObservationId]),
    CONSTRAINT [FK_InspectionObservations_Inspection] FOREIGN KEY ([InspectionId]) REFERENCES [SSO].[Inspections]([InspectionId]),
    CONSTRAINT [FK_InspectionObservations_Responsible] FOREIGN KEY ([ResponsibleUserId]) REFERENCES [Security].[Users]([UserId]),
    CONSTRAINT [CK_InspectionObservations_Status] CHECK ([Status] IN ('Pending', 'InProgress', 'Completed'))
)
GO

-- Imágenes de observaciones
CREATE TABLE [SSO].[ObservationImages] (
    [ImageId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ObservationId] UNIQUEIDENTIFIER NOT NULL,
    [ImageUrl] NVARCHAR(1000) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [SortOrder] INT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_ObservationImages] PRIMARY KEY CLUSTERED ([ImageId]),
    CONSTRAINT [FK_ObservationImages_Observation] FOREIGN KEY ([ObservationId]) REFERENCES [SSO].[InspectionObservations]([ObservationId])
)
GO

-- Evidencias de observaciones
CREATE TABLE [SSO].[ObservationEvidences] (
    [EvidenceId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ObservationId] UNIQUEIDENTIFIER NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [EvidenceUrl] NVARCHAR(1000) NULL,
    [UploadedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UploadedBy] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_ObservationEvidences] PRIMARY KEY CLUSTERED ([EvidenceId]),
    CONSTRAINT [FK_ObservationEvidences_Observation] FOREIGN KEY ([ObservationId]) REFERENCES [SSO].[InspectionObservations]([ObservationId])
)
GO

-- =============================================
-- MÓDULO AUDITORÍAS
-- =============================================

-- Programas de auditoría
CREATE TABLE [SSO].[AuditPrograms] (
    [ProgramId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [ProgramName] NVARCHAR(200) NOT NULL,
    [Year] INT NOT NULL,
    [Standard] NVARCHAR(100) NOT NULL, -- ISO 45001, OHSAS 18001, etc.
    [Description] NVARCHAR(500) NULL,
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_AuditPrograms] PRIMARY KEY CLUSTERED ([ProgramId]),
    CONSTRAINT [FK_AuditPrograms_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [UQ_AuditPrograms_Name] UNIQUE ([TenantId], [ProgramName], [Year])
)
GO

-- Auditorías
CREATE TABLE [SSO].[Audits] (
    [AuditId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [ProgramId] UNIQUEIDENTIFIER NOT NULL,
    [AreaId] UNIQUEIDENTIFIER NOT NULL,
    [AuditCode] NVARCHAR(50) NOT NULL,
    [AuditType] NVARCHAR(50) NOT NULL, -- Internal, External, Certification
    [AuditorUserId] UNIQUEIDENTIFIER NOT NULL,
    [ScheduledDate] DATE NOT NULL,
    [ExecutedDate] DATETIME2 NULL,
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Scheduled', -- Scheduled, InProgress, Completed, Cancelled
    [TotalScore] DECIMAL(5,2) NULL,
    [MaxScore] DECIMAL(5,2) NULL,
    [CompliancePercentage] AS (CASE WHEN [MaxScore] > 0 THEN ([TotalScore] / [MaxScore]) * 100 ELSE 0 END),
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Audits] PRIMARY KEY CLUSTERED ([AuditId]),
    CONSTRAINT [FK_Audits_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [FK_Audits_Program] FOREIGN KEY ([ProgramId]) REFERENCES [SSO].[AuditPrograms]([ProgramId]),
    CONSTRAINT [FK_Audits_Area] FOREIGN KEY ([AreaId]) REFERENCES [SSO].[Areas]([AreaId]),
    CONSTRAINT [FK_Audits_Auditor] FOREIGN KEY ([AuditorUserId]) REFERENCES [Security].[Users]([UserId]),
    CONSTRAINT [UQ_Audits_Code] UNIQUE ([TenantId], [AuditCode])
)
GO

-- Categorías de auditoría (10 tabs)
CREATE TABLE [SSO].[AuditCategories] (
    [CategoryId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [CategoryName] NVARCHAR(200) NOT NULL,
    [CategoryCode] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [SortOrder] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_AuditCategories] PRIMARY KEY CLUSTERED ([CategoryId]),
    CONSTRAINT [UQ_AuditCategories_Code] UNIQUE ([CategoryCode])
)
GO

-- Criterios de evaluación
CREATE TABLE [SSO].[AuditCriteria] (
    [CriteriaId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [CategoryId] UNIQUEIDENTIFIER NOT NULL,
    [CriteriaCode] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [MaxScore] DECIMAL(5,2) NOT NULL,
    [SortOrder] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    CONSTRAINT [PK_AuditCriteria] PRIMARY KEY CLUSTERED ([CriteriaId]),
    CONSTRAINT [FK_AuditCriteria_Category] FOREIGN KEY ([CategoryId]) REFERENCES [SSO].[AuditCategories]([CategoryId]),
    CONSTRAINT [UQ_AuditCriteria_Code] UNIQUE ([CategoryId], [CriteriaCode])
)
GO

-- Evaluaciones de auditoría
CREATE TABLE [SSO].[AuditEvaluations] (
    [EvaluationId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [AuditId] UNIQUEIDENTIFIER NOT NULL,
    [CriteriaId] UNIQUEIDENTIFIER NOT NULL,
    [Score] DECIMAL(5,2) NOT NULL,
    [Observations] NVARCHAR(MAX) NULL,
    [EvidenceRequired] BIT NOT NULL DEFAULT 0,
    [EvaluatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [EvaluatedBy] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_AuditEvaluations] PRIMARY KEY CLUSTERED ([EvaluationId]),
    CONSTRAINT [FK_AuditEvaluations_Audit] FOREIGN KEY ([AuditId]) REFERENCES [SSO].[Audits]([AuditId]),
    CONSTRAINT [FK_AuditEvaluations_Criteria] FOREIGN KEY ([CriteriaId]) REFERENCES [SSO].[AuditCriteria]([CriteriaId]),
    CONSTRAINT [UQ_AuditEvaluations_Criteria] UNIQUE ([AuditId], [CriteriaId])
)
GO

-- Evidencias de auditoría
CREATE TABLE [SSO].[AuditEvidences] (
    [EvidenceId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [EvaluationId] UNIQUEIDENTIFIER NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [EvidenceUrl] NVARCHAR(1000) NOT NULL,
    [UploadedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [UploadedBy] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_AuditEvidences] PRIMARY KEY CLUSTERED ([EvidenceId]),
    CONSTRAINT [FK_AuditEvidences_Evaluation] FOREIGN KEY ([EvaluationId]) REFERENCES [SSO].[AuditEvaluations]([EvaluationId])
)
GO

-- =============================================
-- MÓDULO ACCIDENTES
-- =============================================

-- Accidentes/Incidentes
CREATE TABLE [SSO].[Accidents] (
    [AccidentId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [AccidentCode] NVARCHAR(50) NOT NULL,
    [Type] NVARCHAR(50) NOT NULL, -- Accident, Incident, NearMiss
    [Severity] NVARCHAR(20) NOT NULL, -- Minor, Moderate, Serious, Fatal
    [OccurredAt] DATETIME2 NOT NULL,
    [ReportedAt] DATETIME2 NOT NULL,
    [Shift] NVARCHAR(50) NOT NULL,
    [Location] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [ImmediateCauses] NVARCHAR(MAX) NULL,
    [RootCauses] NVARCHAR(MAX) NULL,
    [WitnessNames] NVARCHAR(500) NULL,
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Reported', -- Reported, UnderInvestigation, Closed
    [InvestigationStartDate] DATETIME2 NULL,
    [InvestigationEndDate] DATETIME2 NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Accidents] PRIMARY KEY CLUSTERED ([AccidentId]),
    CONSTRAINT [FK_Accidents_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [UQ_Accidents_Code] UNIQUE ([TenantId], [AccidentCode]),
    CONSTRAINT [CK_Accidents_Type] CHECK ([Type] IN ('Accident', 'Incident', 'NearMiss')),
    CONSTRAINT [CK_Accidents_Severity] CHECK ([Severity] IN ('Minor', 'Moderate', 'Serious', 'Fatal'))
)
GO

-- Personas involucradas en accidentes
CREATE TABLE [SSO].[AccidentPeople] (
    [PersonId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [AccidentId] UNIQUEIDENTIFIER NOT NULL,
    [PersonType] NVARCHAR(20) NOT NULL, -- Affected, Responsible, Witness
    [UserId] UNIQUEIDENTIFIER NULL,
    [FullName] NVARCHAR(200) NOT NULL,
    [DocumentNumber] NVARCHAR(20) NULL,
    [Age] INT NULL,
    [Gender] NVARCHAR(10) NULL,
    [Position] NVARCHAR(100) NULL,
    [Company] NVARCHAR(200) NULL,
    [AreaId] UNIQUEIDENTIFIER NULL,
    [InjuryType] NVARCHAR(100) NULL,
    [InjuryDescription] NVARCHAR(MAX) NULL,
    [MedicalAttention] BIT NOT NULL DEFAULT 0,
    [LostWorkDays] INT NULL,
    CONSTRAINT [PK_AccidentPeople] PRIMARY KEY CLUSTERED ([PersonId]),
    CONSTRAINT [FK_AccidentPeople_Accident] FOREIGN KEY ([AccidentId]) REFERENCES [SSO].[Accidents]([AccidentId]),
    CONSTRAINT [FK_AccidentPeople_User] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users]([UserId]),
    CONSTRAINT [FK_AccidentPeople_Area] FOREIGN KEY ([AreaId]) REFERENCES [SSO].[Areas]([AreaId]),
    CONSTRAINT [CK_AccidentPeople_Type] CHECK ([PersonType] IN ('Affected', 'Responsible', 'Witness'))
)
GO

-- Imágenes de accidentes
CREATE TABLE [SSO].[AccidentImages] (
    [ImageId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [AccidentId] UNIQUEIDENTIFIER NOT NULL,
    [ImageUrl] NVARCHAR(1000) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [SortOrder] INT NOT NULL DEFAULT 0,
    CONSTRAINT [PK_AccidentImages] PRIMARY KEY CLUSTERED ([ImageId]),
    CONSTRAINT [FK_AccidentImages_Accident] FOREIGN KEY ([AccidentId]) REFERENCES [SSO].[Accidents]([AccidentId])
)
GO

-- =============================================
-- MÓDULO CAPACITACIONES
-- =============================================

-- Capacitaciones
CREATE TABLE [SSO].[Trainings] (
    [TrainingId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NOT NULL,
    [TrainingCode] NVARCHAR(50) NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [TrainingType] NVARCHAR(50) NOT NULL, -- Induction, Periodic, Specific, Emergency
    [Mode] NVARCHAR(20) NOT NULL, -- Presential, Virtual, Mixed
    [InstructorUserId] UNIQUEIDENTIFIER NULL,
    [ExternalInstructor] NVARCHAR(200) NULL,
    [AreaId] UNIQUEIDENTIFIER NOT NULL,
    [ScheduledDate] DATETIME2 NOT NULL,
    [Duration] INT NOT NULL, -- En minutos
    [Location] NVARCHAR(200) NULL,
    [MaxParticipants] INT NULL,
    [Status] NVARCHAR(20) NOT NULL DEFAULT 'Scheduled', -- Scheduled, InProgress, Completed, Cancelled
    [MaterialUrl] NVARCHAR(1000) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] UNIQUEIDENTIFIER NOT NULL,
    [UpdatedAt] DATETIME2 NULL,
    [UpdatedBy] UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_Trainings] PRIMARY KEY CLUSTERED ([TrainingId]),
    CONSTRAINT [FK_Trainings_Tenant] FOREIGN KEY ([TenantId]) REFERENCES [Tenant].[Tenants]([TenantId]),
    CONSTRAINT [FK_Trainings_Instructor] FOREIGN KEY ([InstructorUserId]) REFERENCES [Security].[Users]([UserId]),
    CONSTRAINT [FK_Trainings_Area] FOREIGN KEY ([AreaId]) REFERENCES [SSO].[Areas]([AreaId]),
    CONSTRAINT [UQ_Trainings_Code] UNIQUE ([TenantId], [TrainingCode])
)
GO

-- Participantes de capacitación
CREATE TABLE [SSO].[TrainingParticipants] (
    [ParticipantId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TrainingId] UNIQUEIDENTIFIER NOT NULL,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [AttendanceStatus] NVARCHAR(20) NOT NULL DEFAULT 'Registered', -- Registered, Present, Absent, Excused
    [Score] DECIMAL(5,2) NULL,
    [Passed] BIT NULL,
    [CertificateUrl] NVARCHAR(1000) NULL,
    [Comments] NVARCHAR(500) NULL,
    [RegisteredAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [AttendanceMarkedAt] DATETIME2 NULL,
    CONSTRAINT [PK_TrainingParticipants] PRIMARY KEY CLUSTERED ([ParticipantId]),
    CONSTRAINT [FK_TrainingParticipants_Training] FOREIGN KEY ([TrainingId]) REFERENCES [SSO].[Trainings]([TrainingId]),
    CONSTRAINT [FK_TrainingParticipants_User] FOREIGN KEY ([UserId]) REFERENCES [Security].[Users]([UserId]),
    CONSTRAINT [UQ_TrainingParticipants_User] UNIQUE ([TrainingId], [UserId])
)
GO

-- =============================================
-- TABLAS DE AUDITORÍA
-- =============================================

-- Log de auditoría general
CREATE TABLE [Audit].[AuditLogs] (
    [AuditId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NULL,
    [UserId] UNIQUEIDENTIFIER NULL,
    [UserName] NVARCHAR(100) NULL,
    [Action] NVARCHAR(100) NOT NULL,
    [EntityType] NVARCHAR(100) NOT NULL,
    [EntityId] NVARCHAR(100) NULL,
    [OldValues] NVARCHAR(MAX) NULL,
    [NewValues] NVARCHAR(MAX) NULL,
    [IpAddress] NVARCHAR(50) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [Duration] INT NULL, -- En milisegundos
    [Success] BIT NOT NULL DEFAULT 1,
    [ErrorMessage] NVARCHAR(MAX) NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY CLUSTERED ([AuditId])
)
GO

-- Log de accesos
CREATE TABLE [Audit].[AccessLogs] (
    [LogId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [TenantId] UNIQUEIDENTIFIER NULL,
    [UserId] UNIQUEIDENTIFIER NULL,
    [UserName] NVARCHAR(100) NULL,
    [Action] NVARCHAR(50) NOT NULL, -- Login, Logout, FailedLogin, PasswordChange
    [IpAddress] NVARCHAR(50) NULL,
    [Location] NVARCHAR(200) NULL,
    [DeviceInfo] NVARCHAR(500) NULL,
    [Browser] NVARCHAR(100) NULL,
    [OperatingSystem] NVARCHAR(100) NULL,
    [Success] BIT NOT NULL DEFAULT 1,
    [FailureReason] NVARCHAR(500) NULL,
    [Timestamp] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_AccessLogs] PRIMARY KEY CLUSTERED ([LogId])
)
GO

-- =============================================
-- ÍNDICES
-- =============================================

-- Índices para Tenant
CREATE NONCLUSTERED INDEX [IX_Tenants_IsActive] ON [Tenant].[Tenants] ([IsActive]) INCLUDE ([TenantId], [CompanyName])
GO

-- Índices para Subscriptions
CREATE NONCLUSTERED INDEX [IX_Subscriptions_TenantId] ON [Subscription].[Subscriptions] ([TenantId]) INCLUDE ([Status], [EndDate])
CREATE NONCLUSTERED INDEX [IX_Subscriptions_Status] ON [Subscription].[Subscriptions] ([Status]) INCLUDE ([TenantId], [EndDate])
CREATE NONCLUSTERED INDEX [IX_Subscriptions_EndDate] ON [Subscription].[Subscriptions] ([EndDate]) WHERE [Status] = 'Active'
GO

-- Índices para Users
CREATE NONCLUSTERED INDEX [IX_Users_TenantId] ON [Security].[Users] ([TenantId]) INCLUDE ([IsActive])
CREATE NONCLUSTERED INDEX [IX_Users_Email] ON [Security].[Users] ([NormalizedEmail])
CREATE NONCLUSTERED INDEX [IX_Users_Username] ON [Security].[Users] ([TenantId], [Username])
GO

-- Índices para SSO
CREATE NONCLUSTERED INDEX [IX_Announcements_TenantArea] ON [SSO].[Announcements] ([TenantId], [AreaId]) INCLUDE ([Status])
CREATE NONCLUSTERED INDEX [IX_Announcements_Status] ON [SSO].[Announcements] ([Status]) INCLUDE ([TenantId], [AreaId])
CREATE NONCLUSTERED INDEX [IX_Inspections_TenantArea] ON [SSO].[Inspections] ([TenantId], [AreaId]) INCLUDE ([Status])
CREATE NONCLUSTERED INDEX [IX_Accidents_TenantDate] ON [SSO].[Accidents] ([TenantId], [OccurredAt])
CREATE NONCLUSTERED INDEX [IX_Trainings_TenantArea] ON [SSO].[Trainings] ([TenantId], [AreaId]) INCLUDE ([ScheduledDate])
GO

-- Índices para Audit
CREATE NONCLUSTERED INDEX [IX_AuditLogs_TenantUser] ON [Audit].[AuditLogs] ([TenantId], [UserId], [Timestamp])
CREATE NONCLUSTERED INDEX [IX_AccessLogs_TenantUser] ON [Audit].[AccessLogs] ([TenantId], [UserId], [Timestamp])
GO

-- =============================================
-- DATOS INICIALES
-- =============================================

-- Insertar planes de suscripción
INSERT INTO [Subscription].[Plans] ([PlanId], [PlanName], [PlanType], [Description], [MonthlyPrice], [AnnualPrice], [Currency], [TrialDays], [IsActive], [SortOrder])
VALUES 
(NEWID(), 'Plan Básico', 'Basic', 'Acceso a módulos básicos de SSO', 99.00, 990.00, 'USD', 30, 1, 1),
(NEWID(), 'Plan Estándar', 'Standard', 'Acceso completo con límites estándar', 299.00, 2990.00, 'USD', 30, 1, 2),
(NEWID(), 'Plan Premium', 'Premium', 'Acceso ilimitado con soporte 24/7', 599.00, 5990.00, 'USD', 30, 1, 3)
GO

-- Insertar roles del sistema
INSERT INTO [Security].[Roles] ([RoleId], [RoleName], [NormalizedRoleName], [Description], [IsSystemRole])
VALUES 
(NEWID(), 'SuperAdmin', 'SUPERADMIN', 'Administrador general del sistema', 1),
(NEWID(), 'AdminSSO', 'ADMINSSO', 'Administrador del módulo SSO', 1),
(NEWID(), 'LeaderArea', 'LEADERAREA', 'Líder de área', 1),
(NEWID(), 'UserArea', 'USERAREA', 'Usuario de área', 1),
(NEWID(), 'ContractorResponsible', 'CONTRACTORRESPONSIBLE', 'Responsable de empresa contratista', 1)
GO

-- Insertar permisos del sistema
INSERT INTO [Security].[Permissions] ([PermissionId], [PermissionName], [PermissionCode], [Module], [Description])
VALUES 
-- Permisos de Tenant
(NEWID(), 'Ver Tenants', 'tenant.view', 'Tenant', 'Permite ver la lista de tenants'),
(NEWID(), 'Crear Tenants', 'tenant.create', 'Tenant', 'Permite crear nuevos tenants'),
(NEWID(), 'Editar Tenants', 'tenant.edit', 'Tenant', 'Permite editar tenants'),
(NEWID(), 'Eliminar Tenants', 'tenant.delete', 'Tenant', 'Permite eliminar tenants'),

-- Permisos de Suscripciones
(NEWID(), 'Ver Suscripciones', 'subscription.view', 'Subscription', 'Permite ver suscripciones'),
(NEWID(), 'Gestionar Suscripciones', 'subscription.manage', 'Subscription', 'Permite gestionar suscripciones'),

-- Permisos de Usuarios
(NEWID(), 'Ver Usuarios', 'user.view', 'Security', 'Permite ver usuarios'),
(NEWID(), 'Crear Usuarios', 'user.create', 'Security', 'Permite crear usuarios'),
(NEWID(), 'Editar Usuarios', 'user.edit', 'Security', 'Permite editar usuarios'),
(NEWID(), 'Eliminar Usuarios', 'user.delete', 'Security', 'Permite eliminar usuarios'),

-- Permisos de 9 Pilares
(NEWID(), 'Ver Documentos', 'pillar.view', 'Pillar', 'Permite ver documentos'),
(NEWID(), 'Subir Documentos', 'pillar.upload', 'Pillar', 'Permite subir documentos'),
(NEWID(), 'Editar Documentos', 'pillar.edit', 'Pillar', 'Permite editar documentos'),
(NEWID(), 'Eliminar Documentos', 'pillar.delete', 'Pillar', 'Permite eliminar documentos'),

-- Permisos de Anuncios
(NEWID(), 'Ver Anuncios', 'announcement.view', 'Announcement', 'Permite ver anuncios'),
(NEWID(), 'Crear Anuncios', 'announcement.create', 'Announcement', 'Permite crear anuncios'),
(NEWID(), 'Editar Anuncios', 'announcement.edit', 'Announcement', 'Permite editar anuncios'),
(NEWID(), 'Eliminar Anuncios', 'announcement.delete', 'Announcement', 'Permite eliminar anuncios'),

-- Permisos de Inspecciones
(NEWID(), 'Ver Inspecciones', 'inspection.view', 'Inspection', 'Permite ver inspecciones'),
(NEWID(), 'Crear Inspecciones', 'inspection.create', 'Inspection', 'Permite crear inspecciones'),
(NEWID(), 'Editar Inspecciones', 'inspection.edit', 'Inspection', 'Permite editar inspecciones'),
(NEWID(), 'Eliminar Inspecciones', 'inspection.delete', 'Inspection', 'Permite eliminar inspecciones'),

-- Permisos de Auditorías
(NEWID(), 'Ver Auditorías', 'audit.view', 'Audit', 'Permite ver auditorías'),
(NEWID(), 'Crear Auditorías', 'audit.create', 'Audit', 'Permite crear auditorías'),
(NEWID(), 'Editar Auditorías', 'audit.edit', 'Audit', 'Permite editar auditorías'),
(NEWID(), 'Evaluar Auditorías', 'audit.evaluate', 'Audit', 'Permite evaluar auditorías'),

-- Permisos de Accidentes
(NEWID(), 'Ver Accidentes', 'accident.view', 'Accident', 'Permite ver accidentes'),
(NEWID(), 'Crear Accidentes', 'accident.create', 'Accident', 'Permite crear accidentes'),
(NEWID(), 'Editar Accidentes', 'accident.edit', 'Accident', 'Permite editar accidentes'),
(NEWID(), 'Investigar Accidentes', 'accident.investigate', 'Accident', 'Permite investigar accidentes'),

-- Permisos de Capacitaciones
(NEWID(), 'Ver Capacitaciones', 'training.view', 'Training', 'Permite ver capacitaciones'),
(NEWID(), 'Crear Capacitaciones', 'training.create', 'Training', 'Permite crear capacitaciones'),
(NEWID(), 'Editar Capacitaciones', 'training.edit', 'Training', 'Permite editar capacitaciones'),
(NEWID(), 'Tomar Asistencia', 'training.attendance', 'Training', 'Permite tomar asistencia')
GO

-- Insertar categorías de auditoría (10 tabs)
INSERT INTO [SSO].[AuditCategories] ([CategoryId], [CategoryName], [CategoryCode], [Description], [SortOrder])
VALUES 
(NEWID(), 'Liderazgo y Compromiso', 'LEADERSHIP', 'Evaluación del liderazgo y compromiso de la dirección', 1),
(NEWID(), 'Política de SSO', 'POLICY', 'Evaluación de la política de seguridad y salud ocupacional', 2),
(NEWID(), 'Planificación', 'PLANNING', 'Evaluación de la planificación del sistema SSO', 3),
(NEWID(), 'Apoyo y Recursos', 'SUPPORT', 'Evaluación de recursos y apoyo organizacional', 4),
(NEWID(), 'Operación', 'OPERATION', 'Evaluación de procesos operacionales', 5),
(NEWID(), 'Evaluación del Desempeño', 'PERFORMANCE', 'Evaluación del desempeño del sistema', 6),
(NEWID(), 'Mejora Continua', 'IMPROVEMENT', 'Evaluación de procesos de mejora', 7),
(NEWID(), 'Gestión de Contratistas', 'CONTRACTORS', 'Evaluación de gestión de contratistas', 8),
(NEWID(), 'Preparación para Emergencias', 'EMERGENCY', 'Evaluación de preparación y respuesta ante emergencias', 9),
(NEWID(), 'Cumplimiento Legal', 'COMPLIANCE', 'Evaluación del cumplimiento de requisitos legales', 10)
GO

-- Insertar features por plan
DECLARE @BasicPlanId UNIQUEIDENTIFIER = (SELECT PlanId FROM [Subscription].[Plans] WHERE PlanType = 'Basic')
DECLARE @StandardPlanId UNIQUEIDENTIFIER = (SELECT PlanId FROM [Subscription].[Plans] WHERE PlanType = 'Standard')
DECLARE @PremiumPlanId UNIQUEIDENTIFIER = (SELECT PlanId FROM [Subscription].[Plans] WHERE PlanType = 'Premium')

-- Features Plan Básico
INSERT INTO [Subscription].[PlanFeatures] ([PlanId], [FeatureName], [FeatureCode], [FeatureType], [Value], [Description])
VALUES 
(@BasicPlanId, 'Usuarios Máximos', 'max_users', 'Limit', '5', 'Número máximo de usuarios'),
(@BasicPlanId, 'Almacenamiento', 'storage_gb', 'Limit', '5', 'Almacenamiento en GB'),
(@BasicPlanId, 'Módulo Anuncios', 'module_announcements', 'Module', 'true', 'Acceso al módulo de anuncios'),
(@BasicPlanId, 'Módulo Accidentes', 'module_accidents', 'Module', 'true', 'Acceso básico al módulo de accidentes'),
(@BasicPlanId, 'Reportes Básicos', 'basic_reports', 'Feature', 'true', 'Acceso a reportes básicos'),
(@BasicPlanId, 'Soporte Email', 'email_support', 'Feature', 'true', 'Soporte por correo electrónico')

-- Features Plan Estándar
INSERT INTO [Subscription].[PlanFeatures] ([PlanId], [FeatureName], [FeatureCode], [FeatureType], [Value], [Description])
VALUES 
(@StandardPlanId, 'Usuarios Máximos', 'max_users', 'Limit', '20', 'Número máximo de usuarios'),
(@StandardPlanId, 'Almacenamiento', 'storage_gb', 'Limit', '50', 'Almacenamiento en GB'),
(@StandardPlanId, 'Módulo Anuncios', 'module_announcements', 'Module', 'true', 'Acceso al módulo de anuncios'),
(@StandardPlanId, 'Módulo Accidentes', 'module_accidents', 'Module', 'true', 'Acceso completo al módulo de accidentes'),
(@StandardPlanId, 'Módulo Inspecciones', 'module_inspections', 'Module', 'true', 'Acceso al módulo de inspecciones'),
(@StandardPlanId, 'Módulo Capacitaciones', 'module_trainings', 'Module', 'true', 'Acceso al módulo de capacitaciones'),
(@StandardPlanId, 'Módulo 9 Pilares', 'module_pillars', 'Module', 'true', 'Acceso al módulo de 9 pilares'),
(@StandardPlanId, 'Reportes Avanzados', 'advanced_reports', 'Feature', 'true', 'Acceso a reportes avanzados'),
(@StandardPlanId, 'App Móvil', 'mobile_app', 'Feature', 'true', 'Acceso a aplicación móvil'),
(@StandardPlanId, 'Notificaciones Email', 'email_notifications', 'Feature', 'true', 'Notificaciones por correo'),
(@StandardPlanId, 'Soporte Chat', 'chat_support', 'Feature', 'true', 'Soporte por chat')

-- Features Plan Premium
INSERT INTO [Subscription].[PlanFeatures] ([PlanId], [FeatureName], [FeatureCode], [FeatureType], [Value], [Description])
VALUES 
(@PremiumPlanId, 'Usuarios Máximos', 'max_users', 'Limit', 'unlimited', 'Usuarios ilimitados'),
(@PremiumPlanId, 'Almacenamiento', 'storage_gb', 'Limit', '500', 'Almacenamiento en GB'),
(@PremiumPlanId, 'Todos los Módulos', 'all_modules', 'Module', 'true', 'Acceso a todos los módulos'),
(@PremiumPlanId, 'API Access', 'api_access', 'Feature', 'true', 'Acceso completo a API'),
(@PremiumPlanId, 'Reportes Personalizados', 'custom_reports', 'Feature', 'true', 'Reportes personalizados ilimitados'),
(@PremiumPlanId, 'Integraciones', 'integrations', 'Feature', 'true', 'Integraciones ilimitadas'),
(@PremiumPlanId, 'Notificaciones Real-time', 'realtime_notifications', 'Feature', 'true', 'Notificaciones en tiempo real'),
(@PremiumPlanId, 'Backup Tiempo Real', 'realtime_backup', 'Feature', 'true', 'Respaldo en tiempo real'),
(@PremiumPlanId, 'Soporte 24/7', 'support_247', 'Feature', 'true', 'Soporte 24/7 prioritario'),
(@PremiumPlanId, 'Personalización', 'customization', 'Feature', 'true', 'Personalización completa')
GO

-- =============================================
-- PROCEDIMIENTOS ALMACENADOS
-- =============================================

-- Procedimiento para verificar estado de suscripción
CREATE PROCEDURE [Subscription].[sp_CheckSubscriptionStatus]
    @TenantId UNIQUEIDENTIFIER,
    @IsValid BIT OUTPUT,
    @Message NVARCHAR(500) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Status NVARCHAR(20)
    DECLARE @EndDate DATETIME2
    DECLARE @GracePeriodEndDate DATETIME2
    
    SELECT 
        @Status = Status,
        @EndDate = EndDate,
        @GracePeriodEndDate = GracePeriodEndDate
    FROM [Subscription].[Subscriptions]
    WHERE TenantId = @TenantId
        AND Status IN ('Trial', 'Active', 'Suspended')
    ORDER BY CreatedAt DESC
    
    IF @Status IS NULL
    BEGIN
        SET @IsValid = 0
        SET @Message = 'No se encontró suscripción activa'
        RETURN
    END
    
    IF @Status = 'Suspended' AND @GracePeriodEndDate < GETUTCDATE()
    BEGIN
        SET @IsValid = 0
        SET @Message = 'Suscripción suspendida. Período de gracia expirado'
        RETURN
    END
    
    IF @EndDate < GETUTCDATE() AND @Status = 'Active'
    BEGIN
        -- Actualizar a suspendida
        UPDATE [Subscription].[Subscriptions]
        SET Status = 'Suspended',
            GracePeriodEndDate = DATEADD(DAY, 7, GETUTCDATE()),
            UpdatedAt = GETUTCDATE()
        WHERE TenantId = @TenantId
            AND Status = 'Active'
        
        SET @IsValid = 1
        SET @Message = 'Suscripción en período de gracia'
        RETURN
    END
    
    SET @IsValid = 1
    SET @Message = 'Suscripción válida'
END
GO

-- Procedimiento para verificar límites de features
CREATE PROCEDURE [Subscription].[sp_CheckFeatureLimit]
    @TenantId UNIQUEIDENTIFIER,
    @FeatureCode NVARCHAR(50),
    @CurrentUsage INT,
    @CanUse BIT OUTPUT,
    @RemainingLimit INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Limit INT
    DECLARE @PlanId UNIQUEIDENTIFIER
    
    -- Obtener plan actual
    SELECT @PlanId = p.PlanId
    FROM [Subscription].[Subscriptions] s
    INNER JOIN [Subscription].[Plans] p ON s.PlanId = p.PlanId
    WHERE s.TenantId = @TenantId
        AND s.Status IN ('Trial', 'Active')
    
    -- Obtener límite de la feature
    SELECT @Limit = CASE 
        WHEN Value = 'unlimited' THEN 999999
        WHEN ISNUMERIC(Value) = 1 THEN CAST(Value AS INT)
        ELSE 0
    END
    FROM [Subscription].[PlanFeatures]
    WHERE PlanId = @PlanId
        AND FeatureCode = @FeatureCode
    
    IF @Limit IS NULL OR @Limit = 0
    BEGIN
        SET @CanUse = 0
        SET @RemainingLimit = 0
    END
    ELSE IF @CurrentUsage >= @Limit
    BEGIN
        SET @CanUse = 0
        SET @RemainingLimit = 0
    END
    ELSE
    BEGIN
        SET @CanUse = 1
        SET @RemainingLimit = @Limit - @CurrentUsage
    END
END
GO

-- Función para obtener el TenantId del contexto
CREATE FUNCTION [dbo].[fn_GetCurrentTenantId]()
RETURNS UNIQUEIDENTIFIER
AS
BEGIN
    -- Esta función será implementada para obtener el TenantId del contexto actual
    -- Por ahora retorna NULL, pero en producción usará SESSION_CONTEXT
    RETURN CAST(SESSION_CONTEXT(N'TenantId') AS UNIQUEIDENTIFIER)
END
GO

-- =============================================
-- VISTAS
-- =============================================

-- Vista de suscripciones activas con detalles
CREATE VIEW [Subscription].[vw_ActiveSubscriptions]
AS
SELECT 
    s.SubscriptionId,
    s.TenantId,
    t.CompanyName,
    t.Email AS TenantEmail,
    p.PlanName,
    p.PlanType,
    s.BillingCycle,
    s.Status,
    s.StartDate,
    s.EndDate,
    s.NextBillingDate,
    DATEDIFF(DAY, GETUTCDATE(), s.EndDate) AS DaysRemaining,
    CASE 
        WHEN s.Status = 'Active' AND s.EndDate <= DATEADD(DAY, 7, GETUTCDATE()) THEN 'Warning'
        WHEN s.Status = 'Suspended' THEN 'Critical'
        ELSE 'Normal'
    END AS AlertLevel
FROM [Subscription].[Subscriptions] s
INNER JOIN [Tenant].[Tenants] t ON s.TenantId = t.TenantId
INNER JOIN [Subscription].[Plans] p ON s.PlanId = p.PlanId
WHERE s.Status IN ('Trial', 'Active', 'Suspended')
    AND t.IsActive = 1
GO

-- Vista de uso de features
CREATE VIEW [Subscription].[vw_FeatureUsage]
AS
SELECT 
    fu.TenantId,
    t.CompanyName,
    fu.FeatureCode,
    pf.FeatureName,
    fu.CurrentUsage,
    CASE 
        WHEN pf.Value = 'unlimited' THEN 999999
        WHEN ISNUMERIC(pf.Value) = 1 THEN CAST(pf.Value AS INT)
        ELSE 0
    END AS UsageLimit,
    CASE 
        WHEN pf.Value = 'unlimited' THEN 0
        WHEN ISNUMERIC(pf.Value) = 1 THEN 
            CAST((fu.CurrentUsage * 100.0 / CAST(pf.Value AS INT)) AS DECIMAL(5,2))
        ELSE 0
    END AS UsagePercentage,
    fu.UpdatedAt
FROM [Subscription].[FeatureUsage] fu
INNER JOIN [Tenant].[Tenants] t ON fu.TenantId = t.TenantId
INNER JOIN [Subscription].[Subscriptions] s ON t.TenantId = s.TenantId
INNER JOIN [Subscription].[PlanFeatures] pf ON s.PlanId = pf.PlanId AND fu.FeatureCode = pf.FeatureCode
WHERE s.Status IN ('Trial', 'Active')
    AND pf.FeatureType = 'Limit'
GO

-- =============================================
-- TRIGGERS
-- =============================================

-- Trigger para auditoría automática en tablas principales
CREATE TRIGGER [SSO].[tr_Announcements_Audit]
ON [SSO].[Announcements]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Action NVARCHAR(10)
    DECLARE @UserId UNIQUEIDENTIFIER
    DECLARE @TenantId UNIQUEIDENTIFIER
    
    IF EXISTS(SELECT * FROM inserted) AND EXISTS(SELECT * FROM deleted)
        SET @Action = 'UPDATE'
    ELSE IF EXISTS(SELECT * FROM inserted)
        SET @Action = 'INSERT'
    ELSE
        SET @Action = 'DELETE'
    
    -- Obtener UserId del contexto
    SET @UserId = CAST(SESSION_CONTEXT(N'UserId') AS UNIQUEIDENTIFIER)
    SET @TenantId = CAST(SESSION_CONTEXT(N'TenantId') AS UNIQUEIDENTIFIER)
    
    IF @Action IN ('INSERT', 'UPDATE')
    BEGIN
        INSERT INTO [Audit].[AuditLogs] (TenantId, UserId, Action, EntityType, EntityId, NewValues)
        SELECT 
            @TenantId,
            @UserId,
            @Action,
            'Announcement',
            AnnouncementId,
            (SELECT * FROM inserted WHERE AnnouncementId = i.AnnouncementId FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
        FROM inserted i
    END
    
    IF @Action IN ('UPDATE', 'DELETE')
    BEGIN
        INSERT INTO [Audit].[AuditLogs] (TenantId, UserId, Action, EntityType, EntityId, OldValues)
        SELECT 
            @TenantId,
            @UserId,
            @Action,
            'Announcement',
            AnnouncementId,
            (SELECT * FROM deleted WHERE AnnouncementId = d.AnnouncementId FOR JSON PATH, WITHOUT_ARRAY_WRAPPER)
        FROM deleted d
    END
END
GO

-- =============================================
-- SEGURIDAD - Row Level Security
-- =============================================

-- Habilitar Row Level Security para multitenancy
CREATE SCHEMA [RLS]
GO

-- Función de predicado de seguridad
CREATE FUNCTION [RLS].[fn_TenantAccessPredicate](@TenantId UNIQUEIDENTIFIER)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN SELECT 1 AS fn_result
WHERE 
    @TenantId = CAST(SESSION_CONTEXT(N'TenantId') AS UNIQUEIDENTIFIER)
    OR CAST(SESSION_CONTEXT(N'IsSuperAdmin') AS BIT) = 1
GO

-- Aplicar políticas de seguridad a tablas principales
CREATE SECURITY POLICY [RLS].[TenantSecurityPolicy]
ADD FILTER PREDICATE [RLS].[fn_TenantAccessPredicate]([TenantId]) ON [SSO].[Announcements],
ADD FILTER PREDICATE [RLS].[fn_TenantAccessPredicate]([TenantId]) ON [SSO].[Inspections],
ADD FILTER PREDICATE [RLS].[fn_TenantAccessPredicate]([TenantId]) ON [SSO].[Accidents],
ADD FILTER PREDICATE [RLS].[fn_TenantAccessPredicate]([TenantId]) ON [SSO].[Trainings],
ADD FILTER PREDICATE [RLS].[fn_TenantAccessPredicate]([TenantId]) ON [SSO].[Areas],
ADD FILTER PREDICATE [RLS].[fn_TenantAccessPredicate]([TenantId]) ON [Security].[Users]
WITH (STATE = ON)
GO

-- =============================================
-- JOBS Y MANTENIMIENTO
-- =============================================

-- Procedimiento para limpiar tokens expirados
CREATE PROCEDURE [Security].[sp_CleanupExpiredTokens]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Eliminar refresh tokens expirados
    DELETE FROM [Security].[RefreshTokens]
    WHERE ExpiresAt < GETUTCDATE()
        AND RevokedAt IS NULL
    
    -- Eliminar sesiones expiradas
    DELETE FROM [Security].[UserSessions]
    WHERE ExpiresAt < GETUTCDATE()
        AND EndedAt IS NULL
    
    -- Marcar suscripciones expiradas
    UPDATE [Subscription].[Subscriptions]
    SET Status = 'Expired',
        UpdatedAt = GETUTCDATE()
    WHERE Status = 'Suspended'
        AND GracePeriodEndDate < DATEADD(DAY, -30, GETUTCDATE())
END
GO

-- =============================================
-- PERMISOS DE BASE DE DATOS
-- =============================================

-- Crear roles de base de datos
CREATE ROLE [db_maprosso_admin]
CREATE ROLE [db_maprosso_user]
CREATE ROLE [db_maprosso_readonly]
GO

-- Asignar permisos a roles
GRANT EXECUTE ON SCHEMA::[Subscription] TO [db_maprosso_user]
GRANT EXECUTE ON SCHEMA::[Security] TO [db_maprosso_user]
GRANT SELECT ON SCHEMA::[SSO] TO [db_maprosso_readonly]
GO

-- =============================================
-- FIN DEL SCRIPT
-- =============================================
PRINT 'Script de base de datos MaproSSO ejecutado exitosamente'
GO