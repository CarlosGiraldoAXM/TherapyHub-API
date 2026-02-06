-- Script to insert system menus
-- Execute this script after creating the tables

INSERT INTO [dbo].[Menus] ([Titulo], [Ruta], [Icono], [Orden], [IsActive])
VALUES
    ('Today', '/', 'Calendar', 1, 1),
    ('Clients', '/clients', 'Users', 2, 1),
    ('Staff', '/staff', 'UserCircle', 3, 1),
    ('Create User', '/create-user', 'UserPlus', 4, 1),
    ('User Types', '/user-types', 'UserCog', 5, 1),
    ('Assign Menus', '/assign-menus', 'Settings', 6, 1),
    ('Clinical', '/clinical', 'FolderOpen', 7, 1),
    ('Education', '/education', 'GraduationCap', 8, 1),
    ('Calendar', '/calendar', 'CalendarDays', 9, 1),
    ('Supervision', '/supervision', 'Eye', 10, 1),
    ('Session Notes', '/session-notes', 'FileText', 11, 1),
    ('To-Do', '/todo', 'CheckSquare', 12, 1),
    ('Library', '/library', 'BookOpen', 13, 1),
    ('Messaging', '/messaging', 'MessageSquare', 14, 1),
    ('Documents', '/documents', 'File', 15, 1);
