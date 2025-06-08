# 💹 Desafio Técnico - Corretora de Investimentos

Este projeto consiste em um sistema completo para controle de investimentos, desenvolvido como parte de um desafio técnico. Ele permite o gerenciamento de usuários, ativos e operações financeiras (compra e venda), além de realizar o cálculo de P&L (Profit & Loss) e integrar-se com serviços externos via Kafka para atualizações em tempo real de cotações.

---

# 📌 Requisitos do Desafio

<br />

## 1. Modelagem de Banco Relacional

<br />

A modelagem foi implementada em T-SQL (SQL Server) e está disponível no seguinte script consolidado:  
🔗 [bancoConsolidado.sql](https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Database/bancoConsolidado.sql)

A justificativa dos tipos utilizados em cada campo, os índices aplicados e as triggers configuradas estão documentadas aqui:  
📝 [Explicações e Estrutura do Banco](https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Database/README.md)


<br />

---

<br />

## 2. Índices e Performance

Dois índices foram criados para otimizar as principais consultas:

- `idx_tbOperacoes_usuario_ativo_data`: melhora o desempenho ao consultar operações por usuário e ativo em um intervalo de tempo.
- `idx_tbPosicoes_ativo_usuario`: utilizado nas triggers para acelerar a recuperação das posições por ativo e usuário.

Foram criadas as seguintes triggers no banco de dados:

- `trgAtualizaPL_AposInsertCotacao.sql`: atualiza as posições após a inserção de uma nova cotação para um ativo.
- `trgAtualizaPosicao_AposInsertOperacao.sql`: atualiza as posições após a inserção de um registro de operação.

📄 Scripts dos índices:  
- [tbOperacoes.sql](https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Database/Tables/tbOperacoes.sql)  
- [tbPosicoes.sql](https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Database/Tables/tbPosicoes.sql)

📊 Exemplos de queries otimizadas:  
- [Consultas com índice](https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Database/README.md#-ind%C3%ADces)

⚙️ Triggers desenvolvidas:  
- [Triggers SQL](https://github.com/Maieru/DesafioTecnicoCorretora/tree/main/src/Database/Triggers)

<br />

---

<br />

## 3. Aplicação

A aplicação foi desenvolvida com ASP.NET Core MVC utilizando Entity Framework Core. Ela permite:

- CRUD de usuários e ativos
- Registro de compras de ativos
- Visualização de um resumo financeiro por usuário (P&L, preço médio, total investido, corretagem)

A estrutura segue uma arquitetura em camadas simplificada:
- **Model:** entidades e enums
- **Business:** serviços de domínio e contexto EF
- **Web (Apresentação):** UI MVC com Bootstrap

📦 Código-fonte:  
[Repositório src](https://github.com/Maieru/DesafioTecnicoCorretora/tree/main/src)

<br />

---

<br />

## 4. Lógica de Negócio

Toda a lógica está encapsulada na camada `Business`, incluindo os cálculos financeiros e regras de negócio.

🔹 [OperacaoService.cs](https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Corretora.Bussiness/Services/OperacaoService.cs) – lógica de compra, corretagem, preço médio e totais  
🔹 [CotacaoService.cs](https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Corretora.Bussiness/Services/CotacaoService.cs) – consulta e salvamento de cotações  
🔹 [PosicaoService.cs](https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Corretora.Bussiness/Services/PosicaoService.cs) – cálculo da posição global e por ativo

<br />

---

<br />

## 5. Integração entre Sistemas

Foi implementado um **Worker Service** em .NET que se conecta a um servidor Kafka e consome mensagens do tópico `cotacoes`, contendo atualizações de preços de ativos.

Esse worker é responsável por validar e salvar atualizações de cotações.

⚙️ Código do worker:  
[MonitorCotacoesWorker.cs](https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Corretora.MonitorCotacoes/MonitorCotacoesWorker.cs)

<br />

---

<br />

# ✨ Funcionalidades Extras

Além dos requisitos principais, o projeto também conta com funcionalidades adicionais que contribuem para testes, simulações e validações mais robustas:

### 🧪 Geração de Dados Sintéticos com Python

Foi desenvolvido um script Python para gerar automaticamente dados sintéticos para popular o banco com milhares de registros de usuários, ativos, operações e cotações.  
Esse recurso facilita testes de performance e demonstração de funcionalidades em escala.

🔗 [Script Python para preenchimento do banco](https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Database/Ferramentas/preencheBancoDeDados.py)

<br />

---

<br />

### 📡 Worker Kafka para Geração de Cotações

Além do worker que consome mensagens Kafka, foi desenvolvido um segundo **Worker Service** em .NET que simula o envio contínuo de novas cotações para os ativos cadastrados, publicando mensagens no tópico `cotacoes`.

Esse componente é útil para testes de integração e simulações em tempo real.

🔗 [Gerador de Cotações via Kafka](https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Corretora.MonitorCotacoes/GeradorCotacoesWorker.cs)

