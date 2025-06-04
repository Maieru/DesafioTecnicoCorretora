-- Script com todos os comandos para criar o banco de dados completo
-- Só é necessário executar este script para criar TODO o banco de dados

USE master;

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'corretora')
BEGIN
	DROP DATABASE corretora;
END
GO

CREATE DATABASE corretora
GO

USE corretora
GO

CREATE TABLE [dbo].[tbAtivos] (
    [id]     INT IDENTITY(1,1) PRIMARY KEY,
    [codigo] NVARCHAR(10)      NOT NULL UNIQUE,
    [nome]   NVARCHAR(100)     NOT NULL
);
GO

CREATE TABLE [dbo].[tbCotacoes] (
    [id]             INT IDENTITY(1,1) PRIMARY KEY,
    [ativo_id]       INT               NOT NULL REFERENCES tbAtivos(id),
    [preco_unitario] DECIMAL(10,2)     NOT NULL,
    [data_hora]      DATETIME2          NOT NULL
);
GO

CREATE TABLE [dbo].[tbUsuarios] (
    [id]              UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    [nome]            NVARCHAR(150)    NOT NULL,
    [email]           NVARCHAR(200)    NOT NULL UNIQUE,
    [perc_corretagem] DECIMAL(5,2)     NOT NULL
);
GO

CREATE TABLE [dbo].[tbOperacoes] (
    [id]				INT IDENTITY(1,1)		  PRIMARY KEY,
    [usuario_id]		UNIQUEIDENTIFIER NOT NULL REFERENCES tbUsuarios(id),
    [ativo_id]			INT				 NOT NULL REFERENCES tbAtivos(id),
    [quantidade]		INT				 NOT NULL,
    [preco_unitario]	DECIMAL(10,2)    NOT NULL,
    [tipo_operacao]		CHAR(1)          NOT NULL,
    [corretagem]		DECIMAL(10,2)    NOT NULL,
    [data_hora]			DATETIME2        NOT NULL
);
GO

CREATE TABLE [dbo].[tbPosicoes] (
    [id]            INT IDENTITY(1,1) PRIMARY KEY,
    [usuario_id]    UNIQUEIDENTIFIER  NOT NULL REFERENCES tbUsuarios(id),
    [ativo_id]      INT               NOT NULL REFERENCES tbAtivos(id),
    [quantidade]    INT               NOT NULL,
    [preco_medio]   DECIMAL(10,2)     NOT NULL,
    [pl]            DECIMAL(12,2)     NOT NULL
);
GO