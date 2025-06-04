CREATE TABLE [dbo].[tbPosicoes] (
    [id]            INT IDENTITY(1,1) PRIMARY KEY,
    [usuario_id]    UNIQUEIDENTIFIER  NOT NULL REFERENCES tbUsuarios(id),
    [ativo_id]      INT               NOT NULL REFERENCES tbAtivos(id),
    [quantidade]    INT               NOT NULL,
    [preco_medio]   DECIMAL(10,2)     NOT NULL,
    [pl]            DECIMAL(12,2)     NOT NULL
);