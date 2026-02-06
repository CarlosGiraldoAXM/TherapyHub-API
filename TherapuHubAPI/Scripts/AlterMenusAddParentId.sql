-- Script to add ParentId to Menus table for hierarchical menus (containers and submenus)
-- Execute this script in the TherapuHub database

-- Add ParentId column (nullable: NULL = top-level menu, non-null = child of that menu)
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[Menus]') AND name = 'ParentId'
)
BEGIN
    ALTER TABLE [dbo].[Menus]
    ADD [ParentId] INT NULL;

    PRINT 'Column ParentId added to Menus.';
END
ELSE
BEGIN
    PRINT 'Column ParentId already exists on Menus.';
END
GO

-- Add foreign key to self-reference (parent menu)
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_Menus_Parent' AND parent_object_id = OBJECT_ID(N'[dbo].[Menus]')
)
BEGIN
    ALTER TABLE [dbo].[Menus]
    ADD CONSTRAINT [FK_Menus_Parent]
    FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Menus]([Id]);

    PRINT 'Foreign key FK_Menus_Parent created.';
END
ELSE
BEGIN
    PRINT 'Foreign key FK_Menus_Parent already exists.';
END
GO

-- Optional: create index for filtering by parent
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Menus_ParentId' AND object_id = OBJECT_ID(N'[dbo].[Menus]'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Menus_ParentId]
    ON [dbo].[Menus] ([ParentId]);
    PRINT 'Index IX_Menus_ParentId created.';
END
GO

-- Optional: insert "Menu Admin" menu so you can assign it to a role and access /menu-admin
IF NOT EXISTS (SELECT 1 FROM [dbo].[Menus] WHERE [Ruta] = '/menu-admin')
BEGIN
    INSERT INTO [dbo].[Menus] ([Titulo], [Ruta], [Icono], [Orden], [IsActive], [ParentId])
    VALUES (N'Menu Admin', '/menu-admin', 'Sliders', 0, 1, NULL);
    PRINT 'Menu Admin entry added.';
END
GO

PRINT 'AlterMenusAddParentId script completed successfully.';
