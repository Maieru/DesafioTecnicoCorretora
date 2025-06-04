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