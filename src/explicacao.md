# 🚀 Como criar o banco de dados

Para criar todas as tabelas e o banco de dados `corretora`, basta executar o script **bancoConsolidado.sql** localizado na raiz dessa pasta.

Esse arquivo contém todos os comandos necessários para criar o banco e suas tabelas de forma consolidada e automatizada. **Os scripts foram feitos para SQL Server**.


# 📋 Tabelas e Estrutura

### `tbUsuarios`

| Coluna            | Tipo               | Explicação                                                                                                                                                      |
|-------------------|--------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `id`              | `UNIQUEIDENTIFIER` | Chave primária com `NEWID()` automático. Usar o ID como `INT IDENTITY(1,1)` também seria uma solução, mas um GUID garante que ele será globalmente único e torna mais difícil descobrir qual ID está associado a qual usuário (além de ser mais chique :P). |
| `nome`            | `NVARCHAR(150)`    | Nome completo do usuário. Dificilmente existirão usuários com nome e sobrenome maiores que 150 caracteres.                                                     |
| `email`           | `NVARCHAR(200)`    | E-mail do usuário. Usar `NVARCHAR(MAX)` seria menos performático, mas permitiria qualquer tamanho de e-mail.                                                   |
| `perc_corretagem` | `DECIMAL(5,2)`     | Percentual de corretagem (ex: 1.50 = 1,5%). Dependendo do negócio, talvez seja necessário o uso de mais casas decimais.                                        |

---

### `tbAtivos`

| Coluna  | Tipo            | Explicação                                                                                                                                   |
|---------|-----------------|----------------------------------------------------------------------------------------------------------------------------------------------|
| `id`    | `INT IDENTITY`  | Identificador do ativo. Um `UNIQUEIDENTIFIER` poderia ser utilizado também, mas, por se tratar de um tipo de dado que considero menos sensível, preferi o uso de um inteiro. |
| `codigo`| `NVARCHAR(10)`  | Código do ativo (ex: PETR4). Os que encontrei têm de 5 a 6 caracteres, então acredito que 10 seja suficiente.                               |
| `nome`  | `NVARCHAR(100)` | Nome completo do ativo. Dificilmente existirão ativos com nome maior que 100 caracteres.                                                   |

---

### `tbCotacoes`

| Coluna           | Tipo            | Explicação                                                                                                                                         |
|------------------|-----------------|----------------------------------------------------------------------------------------------------------------------------------------------------|
| `id`             | `INT IDENTITY`  | Identificador da cotação. Um `UNIQUEIDENTIFIER` poderia ser utilizado também, mas, por se tratar de um tipo de dado que considero menos sensível, preferi o uso de um inteiro. |
| `ativo_id`       | `INT`           | FK para `tbAtivos(id)`. Precisa ser igual ao tipo referenciado.                                                                                   |
| `preco_unitario` | `DECIMAL(10,2)` | Preço atual da ação ou ativo. Dependendo do negócio, talvez seja necessário o uso de mais casas decimais.                                        |
| `data_hora`      | `DATETIME2`     | Data/hora da cotação. O `DATETIME` também poderia ser utilizado, mas o MSDN recomenda o uso de `DATETIME2` para novas implementações.            |

---

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

---

### `tbPosicoes`

| Coluna         | Tipo                | Explicação                                                                                                                                         |
|----------------|---------------------|----------------------------------------------------------------------------------------------------------------------------------------------------|
| `id`           | `INT IDENTITY`      | Identificador da posição. Um `UNIQUEIDENTIFIER` poderia ser utilizado também, mas, por se tratar de um tipo de dado que considero menos sensível, preferi o uso de um inteiro. |
| `usuario_id`   | `UNIQUEIDENTIFIER`  | FK para `tbUsuarios(id)`. Precisa ser igual ao tipo referenciado.                                                                                 |
| `ativo_id`     | `INT`               | FK para `tbAtivos(id)`. Precisa ser igual ao tipo referenciado.                                                                                   |
| `quantidade`   | `INT`               | Quantidade atual do ativo em carteira. Se o Elon Musk for investir utilizando a corretora, talvez seja necessário usar um `BIGINT`.              |
| `preco_medio`  | `DECIMAL(10,2)`     | Preço médio de aquisição. Dependendo do negócio, talvez seja necessário o uso de mais casas decimais.                                            |
| `pl`           | `DECIMAL(12,2)`     | Lucro/Prejuízo atual (Profit & Loss / P&L). Dependendo do negócio, talvez seja necessário o uso de mais casas decimais.                          |
