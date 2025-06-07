CREATE TRIGGER trgAtualizaPL_AposInsertCotacao
ON tbCotacoes
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ativo_id   INT;
    DECLARE @novo_preco DECIMAL(10,2);

    -- Aqui estou considerando que somente vou inserir uma operação por vez
    SELECT  
        @ativo_id = ativo_id,
        @novo_preco = preco_unitario
    FROM inserted;

    UPDATE tbPosicoes
    SET pl = (@novo_preco - preco_medio) * quantidade
    WHERE ativo_id = @ativo_id;
END;
GO