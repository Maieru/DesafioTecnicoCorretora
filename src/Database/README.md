# 🚀 Como criar o banco de dados

Para criar todas as tabelas e o banco de dados `corretora`, basta executar o script **bancoConsolidado.sql** localizado na raiz dessa pasta.

Esse arquivo contém todos os comandos necessários para criar o banco e suas tabelas de forma consolidada e automatizada. **Os scripts foram feitos para SQL Server**.

<br/>
<br/>

# 📋 Tabelas e Estrutura

### `tbUsuarios`

| Coluna            | Tipo               | Explicação                                                                                                                                                      |
|-------------------|--------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `id`              | `UNIQUEIDENTIFIER` | Chave primária com `NEWID()` automático. Usar o ID como `INT IDENTITY(1,1)` também seria uma solução, mas um GUID garante que ele será globalmente único e torna mais difícil descobrir qual ID está associado a qual usuário (além de ser mais chique :P). |
| `nome`            | `NVARCHAR(150)`    | Nome completo do usuário. Dificilmente existirão usuários com nome e sobrenome maiores que 150 caracteres.                                                     |
| `email`           | `NVARCHAR(200)`    | E-mail do usuário. Usar `NVARCHAR(MAX)` seria menos performático, mas permitiria qualquer tamanho de e-mail.                                                   |
| `perc_corretagem` | `DECIMAL(5,2)`     | Percentual de corretagem (ex: 1.50 = 1,5%). Dependendo do negócio, talvez seja necessário o uso de mais casas decimais.                                        |

<br/>

---

<br/>

### `tbAtivos`

| Coluna  | Tipo            | Explicação                                                                                                                                   |
|---------|-----------------|----------------------------------------------------------------------------------------------------------------------------------------------|
| `id`    | `INT IDENTITY`  | Identificador do ativo. Um `UNIQUEIDENTIFIER` poderia ser utilizado também, mas, por se tratar de um tipo de dado que considero menos sensível, preferi o uso de um inteiro. |
| `codigo`| `NVARCHAR(10)`  | Código do ativo (ex: PETR4). Os que encontrei têm de 5 a 6 caracteres, então acredito que 10 seja suficiente.                               |
| `nome`  | `NVARCHAR(100)` | Nome completo do ativo. Dificilmente existirão ativos com nome maior que 100 caracteres.                                                   |

<br/>

---

<br/>

### `tbCotacoes`

| Coluna           | Tipo            | Explicação                                                                                                                                         |
|------------------|-----------------|----------------------------------------------------------------------------------------------------------------------------------------------------|
| `id`             | `INT IDENTITY`  | Identificador da cotação. Um `UNIQUEIDENTIFIER` poderia ser utilizado também, mas, por se tratar de um tipo de dado que considero menos sensível, preferi o uso de um inteiro. |
| `ativo_id`       | `INT`           | FK para `tbAtivos(id)`. Precisa ser igual ao tipo referenciado.                                                                                   |
| `preco_unitario` | `DECIMAL(10,2)` | Preço atual da ação ou ativo. Dependendo do negócio, talvez seja necessário o uso de mais casas decimais.                                        |
| `data_hora`      | `DATETIME2`     | Data/hora da cotação. O `DATETIME` também poderia ser utilizado, mas o MSDN recomenda o uso de `DATETIME2` para novas implementações.            |

### ⚡ Trigger

A trigger `trgAtualizaPL_AposInsertCotacao` foi criada na tabela `tbCotacoes` com o objetivo de atualizar automaticamente o **P&L (Profit and Loss)** das posições dos usuários sempre que uma nova cotação for registrada para um ativo.

Quando um novo registro é inserido em `tbCotacoes`, a trigger:

- Identifica o ativo associado à nova cotação.
- Atualiza o campo `pl` de todas as posições relacionadas a esse ativo.

<br/>

---

<br/>

### `tbOperacoes`

| Coluna           | Tipo                | Explicação                                                                                                                                         |
|------------------|---------------------|----------------------------------------------------------------------------------------------------------------------------------------------------|
| `id`             | `INT IDENTITY`      | Identificador da operação. Um `UNIQUEIDENTIFIER` poderia ser utilizado também, mas, por se tratar de um tipo de dado que considero menos sensível, preferi o uso de um inteiro. |
| `usuario_id`     | `UNIQUEIDENTIFIER`  | FK para `tbUsuarios(id)`. Precisa ser igual ao tipo referenciado.                                                                                 |
| `ativo_id`       | `INT`               | FK para `tbAtivos(id)`. Precisa ser igual ao tipo referenciado.                                                                                   |
| `quantidade`     | `INT`               | Quantidade de ativos envolvidos na operação. Eu acho que não tem como negociar frações de ativos, por isso utilizei o inteiro.                   |
| `preco_unitario` | `DECIMAL(10,2)`     | Preço por unidade do ativo. Dependendo do negócio, talvez seja necessário o uso de mais casas decimais.                                          |
| `tipo_operacao`  | `CHAR(1)`           | Tipo de operação. Pensei em armazenar os tipos de operação como valores inteiros (exemplo: 0 => compra; 1 => venda).                             |
| `corretagem`     | `DECIMAL(10,2)`     | Valor da corretagem aplicada à operação. Dependendo do negócio, talvez seja necessário o uso de mais casas decimais.                             |
| `data_hora`      | `DATETIME2`         | Data e hora da operação. O `DATETIME` também poderia ser utilizado, mas o MSDN recomenda o uso de `DATETIME2` para novas implementações.         |

### 🔍 Indíces

Foi criado o índice `idx_tbOperacoes_usuario_ativo_data` para melhorar a performance de consultas das operações de um determinado usuário e ativo em um período de tempo. 
O índice evita a realização de table scans na tabela de operações em busca de um usuário ou ativo. As demais colunas da tabela de operações não foram incluídas no índice por que isso penalizaria a
operação de inserção e não traria mudanças tão significativas nas consultas.
Um exemplo de query que utilizaria esse índice seria:

<br/>

```sql
DECLARE @usuario_id UNIQUEIDENTIFIER = 'meu usuário'
DECLARE @ativo_id INT = 1
DECLARE @numero_dias INT = 30

SELECT *
FROM tbOperacoes
WHERE usuario_id = @usuario_id
  AND ativo_id = @ativo_id
  AND data_hora > DATEADD(DAY, -@numero_dias, GETDATE())
ORDER BY data_hora DESC;
``````

### ⚡ Triggers

A trigger `trgAtualizaPosicao_AposInsertOperacao` foi criada na tabela `tbOperacoes` com o objetivo de manter a tabela `tbPosicoes` sempre sincronizada com as movimentações de compra e venda realizadas pelos usuários.

Sempre que um novo registro é inserido em `tbOperacoes`, a trigger:

- Verifica se já existe uma posição do usuário para o ativo negociado.
- Caso exista:
  - Se a operação for uma compra ('0'), atualiza a quantidade total e recalcula o preço médio ponderado.
  - Se for uma venda ('1'), reduz a quantidade da posição. O preço médio permanece inalterado.
- Caso a posição ainda não exista:
  - Insere um novo registro em tbPosicoes com os dados iniciais (quantidade, preço médio, etc.).
- Em ambos os casos, busca a cotação mais recente do ativo e atualiza o P&L (lucro/prejuízo) da posição com base na diferença entre o preço atual e o preço médio.

<br/>

---

<br/>

### `tbPosicoes`

| Coluna         | Tipo                | Explicação                                                                                                                                         |
|----------------|---------------------|----------------------------------------------------------------------------------------------------------------------------------------------------|
| `id`           | `INT IDENTITY`      | Identificador da posição. Um `UNIQUEIDENTIFIER` poderia ser utilizado também, mas, por se tratar de um tipo de dado que considero menos sensível, preferi o uso de um inteiro. |
| `usuario_id`   | `UNIQUEIDENTIFIER`  | FK para `tbUsuarios(id)`. Precisa ser igual ao tipo referenciado.                                                                                 |
| `ativo_id`     | `INT`               | FK para `tbAtivos(id)`. Precisa ser igual ao tipo referenciado.                                                                                   |
| `quantidade`   | `INT`               | Quantidade atual do ativo em carteira. Se o Elon Musk for investir utilizando a corretora, talvez seja necessário usar um `BIGINT`.              |
| `preco_medio`  | `DECIMAL(10,2)`     | Preço médio de aquisição. Dependendo do negócio, talvez seja necessário o uso de mais casas decimais.                                            |
| `pl`           | `DECIMAL(12,2)`     | Lucro/Prejuízo atual (Profit & Loss / P&L). Dependendo do negócio, talvez seja necessário o uso de mais casas decimais.                          |

### 🔍 Indíces

Foi criado o índice `idx_tbPosicoes_ativo_usuario` para melhorar a performance das triggers `trgAtualizaPosicao_AposInsertOperacao` e `trgAtualizaPL_AposInsertCotacao`. O índice evita a realização de table scan na busca das posições de um determinado ativo e/ou usuário. Foram incluídas todas as colunas da tabela porque imagino que serão feitas muito mais consultas do que inserções ou atualizações nessa tabela.
