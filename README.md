# 💹 Desafio Técnico - Corretora de Investimentos

Este projeto é um sistema completo de controle de investimentos, desenvolvido como parte de um desafio técnico. Ele permite gerenciar usuários, ativos, operações financeiras (compra e venda), cálculo de P&L (Profit & Loss), e integrações em tempo real com serviços externos via Kafka.

<br />

---

<br />

# 📌 Requisitos do Desafio

<br />

## 1. Modelagem de Banco Relacional

O banco de dados foi feito em TSQL e se encontra no seguinte link:
https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Database/bancoConsolidado.sql
<br />
<br />
A descrição e justificativa de cada campo, índices criados e triggers se encontram no link: 
https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Database/README.md

<br />

## 2. Índices e Performance

Foram criados dois índices visando a performance de consultas de operações e posições. O índice `idx_tbOperacoes_usuario_ativo_data` visa melhorar a performance de consultas das operações de um determinado usuário e ativo em um período de tempo. O índice `idx_tbPosicoes_ativo_usuario` visa melhorar a performance das triggers `trgAtualizaPosicao_AposInsertOperacao` e `trgAtualizaPL_AposInsertCotacao`, que atualizam os dados de posição após uma nova operação ou uma troca na cotação.

Os índices podem ser consultados nos seguintes links:
https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Database/Tables/tbOperacoes.sql
https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Database/Tables/tbPosicoes.sql
<br />
<br />
Os exemplos de querys otimizadas pelos indices podem ser vistos em:
https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Database/README.md#-ind%C3%ADces
https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Database/README.md#-ind%C3%ADces
<br />
<br />
As triggers desenvolvidas podem ser visualizadas no seguinte link:
https://github.com/Maieru/DesafioTecnicoCorretora/tree/main/src/Database/Triggers

<br />

## 3. Aplicação

Foi desenvolvido uma aplicação aspnet core MVC que permite a criação de usuários, ativos e compras de ativos. Além disso, também é possível consultar um resumo das operações do usuário, que mostra dados como o preço médio por ativo, total investido por ativo, total de corretagem e P&L por papel e global. Foi utilizado o Entity Framework como ferramenta de ORM. Foi utilizado uma versão simplificada da arquitetura de camadas, definindo uma camada de Model (onde se encontram as models e enumeradores), uma de Bussiness (onde são armazenados os serviços e o contexto de banco de dados) e uma de apresentação (no caso, o aplicativo aspnet MVC).

O código fonte desenvolvido se encontra no seguinte link:
https://github.com/Maieru/DesafioTecnicoCorretora/tree/main/src

<br />

## 4. Lógica de Negócio

A lógica de negócio foi concentrada na camada de Bussiness do sistema. Lá estão encontradas todos as consultas complexas.

O serviço de cotação (onde se recupera a cotação de um ativo e salva uma nova cotação) pode ser encontrado no seguinte link:
https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Corretora.Bussiness/Services/CotacaoService.cs
<br />
<br />
O serviço de operação (onde se recupera o total investido por ativo, o total de vendas por ativo, o total de corretagem, a lógica de compra e o cálculo de preço médio) pode ser encontrado no seguinte link:
https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Corretora.Bussiness/Services/OperacaoService.cs
<br />
<br />
O serviço de posição (onde se recupera a posição global e por papel de um usuário) pode ser encontrado no seguinte link:
https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Corretora.Bussiness/Services/PosicaoService.cs

<br />

## 5. Integração entre Sistemas

Foi desenvolvido um Worker Service que consome uma fila de publicações feita em um servidor Kafka. Essas publicações possuem informações sobre novos valores de cotação para ativos.

O código do monitor de cotações pode ser consultado no seguinte link:
https://github.com/Maieru/DesafioTecnicoCorretora/blob/main/src/Corretora.MonitorCotacoes/MonitorCotacoesWorker.cs
