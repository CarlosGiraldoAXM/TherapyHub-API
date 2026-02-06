-- Script to create Menus and User Type assignment tables
-- Execute this script in the TherapuHub database

-- Menus table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Menus]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Menus] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Titulo] NVARCHAR(100) NOT NULL,
        [Ruta] NVARCHAR(255) NOT NULL,
        [Icono] NVARCHAR(100) NULL,
        [Orden] INT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Menus] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    PRINT 'Menus table created successfully';
END
ELSE
BEGIN
    PRINT 'Menus table already exists';
END
GO

-- UserTypeMenus table (Many-to-many relationship table)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserTypeMenus]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserTypeMenus] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [UserTypeId] INT NOT NULL,
        [MenuId] INT NOT NULL,
        [FechaAsignacion] DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_TipoUsuarioMenu] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    
    PRINT 'UserTypeMenus table created successfully';
END
ELSE
BEGIN
    PRINT 'UserTypeMenus table already exists';
END
GO

-- Insert system menus
-- Note: If menus already exist, this script will not insert them again
IF NOT EXISTS (SELECT * FROM [dbo].[Menus] WHERE [Ruta] = '/')
BEGIN
    INSERT INTO [dbo].[Menus] ([Titulo], [Ruta], [Icono], [Orden], [IsActive])
    VALUES
        ('Today', '/', 'Calendar', 1, 1),
        ('Clients', '/clients', 'Users', 2, 1),
        ('Staff', '/staff', 'UserCircle', 3, 1),
        ('Create User', '/create-user', 'UserPlus', 4, 1),
        ('Assign Menus', '/assign-menus', 'Settings', 5, 1),
        ('Clinical', '/clinical', 'FolderOpen', 6, 1),
        ('Education', '/education', 'GraduationCap', 7, 1),
        ('Calendar', '/calendar', 'CalendarDays', 8, 1),
        ('Supervision', '/supervision', 'Eye', 9, 1),
        ('Session Notes', '/session-notes', 'FileText', 10, 1),
        ('To-Do', '/todo', 'CheckSquare', 11, 1),
        ('Library', '/library', 'BookOpen', 12, 1),
        ('Messaging', '/messaging', 'MessageSquare', 13, 1),
        ('Documents', '/documents', 'File', 14, 1);
    
    PRINT 'Menus inserted successfully';
END
ELSE
BEGIN
    PRINT 'Menus already exist in the database';
END
GO

-- Create indexes to improve performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TipoUsuarioMenu_TipoUsuarioId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_TipoUsuarioMenu_TipoUsuarioId] 
    ON [dbo].[UserTypeMenus] ([UserTypeId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TipoUsuarioMenu_MenuId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_TipoUsuarioMenu_MenuId] 
    ON [dbo].[UserTypeMenus] ([MenuId]);
END
GO

PRINT 'Script executed successfully';
