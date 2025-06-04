CREATE TABLE [dbo].[tbCotacoes] (
    [id]             INT IDENTITY(1,1) PRIMARY KEY,
    [ativo_id]       INT               NOT NULL REFERENCES tbAtivos(id),
    [preco_unitario] DECIMAL(10,2)     NOT NULL,
    [data_hora]      DATETIME2         NOT NULL
);