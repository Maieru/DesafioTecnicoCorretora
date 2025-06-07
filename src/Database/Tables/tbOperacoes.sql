CREATE TABLE [dbo].[tbOperacoes] (
    [id]				INT IDENTITY(1,1)		  PRIMARY KEY,
    [usuario_id]		UNIQUEIDENTIFIER NOT NULL REFERENCES tbUsuarios(id),
    [ativo_id]			INT				 NOT NULL REFERENCES tbAtivos(id),
    [quantidade]		INT				 NOT NULL,
    [preco_unitario]	DECIMAL(10,2)    NOT NULL,
    [tipo_operacao]		INT              NOT NULL,
    [corretagem]		DECIMAL(10,2)    NOT NULL,
    [data_hora]			DATETIME2        NOT NULL
);
GO

CREATE NONCLUSTERED INDEX idx_tbOperacoes_usuario_ativo_data
ON tbOperacoes (usuario_id, ativo_id, data_hora)
GO