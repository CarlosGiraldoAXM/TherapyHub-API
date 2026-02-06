-- Script to insert user types
-- Execute this script after creating the UserTypes table

INSERT INTO [dbo].[UserTypes] ([Nombre], [Descripcion], [IsActive])
VALUES
    ('Administrator', 'System administrator with full access to all features and settings', 1),
    ('Supervisor', 'Supervisor role with access to staff management and clinical oversight', 1),
    ('RBT', 'Registered Behavior Technician with access to client sessions and notes', 1),
    ('BCBA', 'Board Certified Behavior Analyst with clinical and supervisory access', 1),
    ('Parent', 'Parent role with limited access to view their child''s information', 1);
