CREATE TRIGGER trgAtualizaPosicao_AposInsertOperacao
ON tbOperacoes
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @tipoCompra			INT = 0;
	DECLARE @tipoVenda			INT = 1;

    DECLARE @usuario_id			UNIQUEIDENTIFIER;
    DECLARE @ativo_id			INT;
    DECLARE @quantidade			INT;
    DECLARE @preco_unitario		DECIMAL(10,2);
    DECLARE @tipo				CHAR(1);
    DECLARE @corretagem			DECIMAL(10,2);
    DECLARE @preco_medio_atual	DECIMAL(10,2);
    DECLARE @quantidade_atual	INT;
    DECLARE @preco_cotacao		DECIMAL(10,2);
    DECLARE @pl					DECIMAL(12,2);

    -- Aqui estou considerando que somente vou inserir uma operação por vez
    SELECT 
        @usuario_id = usuario_id,
        @ativo_id = ativo_id,
        @quantidade = quantidade,
        @preco_unitario = preco_unitario,
        @tipo = tipo_operacao,
        @corretagem = corretagem
    FROM inserted;

    SELECT 
        @quantidade_atual = quantidade,
        @preco_medio_atual = preco_medio
    FROM tbPosicoes
    WHERE usuario_id = @usuario_id AND ativo_id = @ativo_id;

    IF EXISTS (
        SELECT 1 FROM tbPosicoes 
        WHERE usuario_id = @usuario_id AND ativo_id = @ativo_id
    )
    BEGIN
        IF @tipo = @tipoCompra
        BEGIN
            DECLARE @novo_total_valor DECIMAL(18,4);
            DECLARE @novo_total_qtd INT;

            -- Essa parte é meio complicadinha, mas o que acontece é que eu calculo o total do valor da posição atual (preço médio * quantidade atual)
            -- e somo com o valor da nova operação (preço unitário * quantidade + corretagem)
            SET @novo_total_valor = (@preco_medio_atual * @quantidade_atual) + (@preco_unitario * @quantidade + @corretagem);
            SET @novo_total_qtd = @quantidade_atual + @quantidade;

            SET @preco_medio_atual = @novo_total_valor / @novo_total_qtd;
            SET @quantidade_atual = @novo_total_qtd;
        END
        ELSE IF @tipo = @tipoVenda
        BEGIN
            SET @quantidade_atual = @quantidade_atual - @quantidade;
        END

        SELECT TOP 1 @preco_cotacao = preco_unitario
        FROM tbCotacoes
        WHERE ativo_id = @ativo_id
        ORDER BY data_hora DESC;

        SET @pl = (@preco_cotacao - @preco_medio_atual) * @quantidade_atual;

        UPDATE tbPosicoes
        SET quantidade = @quantidade_atual,
            preco_medio = @preco_medio_atual,
            pl = @pl
        WHERE usuario_id = @usuario_id AND ativo_id = @ativo_id;
    END
    ELSE
    BEGIN
        IF @tipo = @tipoCompra
        BEGIN
            SET @preco_medio_atual = (@preco_unitario * @quantidade + @corretagem) / @quantidade;
            SET @quantidade_atual = @quantidade;

            SELECT TOP 1 @preco_cotacao = preco_unitario
            FROM tbCotacoes
            WHERE ativo_id = @ativo_id
            ORDER BY data_hora DESC;

            SET @pl = (@preco_cotacao - @preco_medio_atual) * @quantidade;

            INSERT INTO tbPosicoes (usuario_id, ativo_id, quantidade, preco_medio, pl)
            VALUES (@usuario_id, @ativo_id, @quantidade, @preco_medio_atual, @pl);
        END
    END
END
GO