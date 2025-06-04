CREATE TABLE [dbo].[tbAtivos] (
    [id]     INT IDENTITY(1,1) PRIMARY KEY,
    [codigo] NVARCHAR(10)      NOT NULL UNIQUE,
    [nome]   NVARCHAR(100)     NOT NULL
);