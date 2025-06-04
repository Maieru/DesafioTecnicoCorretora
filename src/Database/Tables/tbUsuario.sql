CREATE TABLE [dbo].[tbUsuarios] (
    [id]              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [nome]            NVARCHAR(150)    NOT NULL,
    [email]           NVARCHAR(200)    NOT NULL UNIQUE,
    [perc_corretagem] DECIMAL(5,2)     NOT NULL
);