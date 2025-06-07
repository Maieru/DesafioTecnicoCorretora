using Confluent.Kafka;
using Corretora.Bussiness.Database;
using Corretora.Bussiness.Services;
using Corretora.Model.DTO;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace Corretora.MonitorCotacoes
{
    public class MonitorCotacoesWorker : BackgroundService
    {
        private readonly ILogger<MonitorCotacoesWorker> _logger;
        private readonly ConsumerConfig _configKafka;
        private readonly CorretoraContext _corretoraContext;
        private readonly IMemoryCache _cache;

        public MonitorCotacoesWorker(ILogger<MonitorCotacoesWorker> logger, ConsumerConfig configuracoesKafka,
            CorretoraContext corretoraContext, IMemoryCache memoryCache)
        {
            _logger = logger;
            _configKafka = configuracoesKafka;
            _corretoraContext = corretoraContext;
            _cache = memoryCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var kafkaConsumer = new ConsumerBuilder<Ignore, string>(_configKafka).Build();
            kafkaConsumer.Subscribe("cotacoes");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = kafkaConsumer.Consume(stoppingToken);
                    var json = result.Message.Value;
                    _logger.LogInformation($"Iniciando processamento de {json}");

                    var cotacao = JsonSerializer.Deserialize<CotacaoDTO>(json);

                    if (cotacao != null)
                        await ProcessarCotacaoAsync(cotacao);
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, "Erro ao consumir mensagem Kafka.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar cotação.");
                }
            }
        }

        private async Task ProcessarCotacaoAsync(CotacaoDTO cotacao)
        {
            const int maxTentativas = 3;
            int tentativa = 0;
            Exception? ultimaExcecao = null;

            if (!await ValidaCotacao(cotacao))
            {
                _logger.LogWarning($"Cotação inválida para o ativo com ID {cotacao.AtivoId}. A cotação não será salva.");
                return;
            }

            while (tentativa < maxTentativas)
            {
                try
                {
                    var cotacaoService = new CotacaoService(_corretoraContext);
                    await cotacaoService.SalvaCotacao(cotacao.PrecoUnitario, cotacao.AtivoId);

                    return; // Se a validação e o salvamento forem bem-sucedidos, sai do loop de tentativas
                }
                catch (Exception ex)
                {
                    tentativa++;
                    ultimaExcecao = ex;
                    _logger.LogWarning(ex, $"Erro ao tentar processar cotação. Tentativa {tentativa}/{maxTentativas}.");

                    if (tentativa < maxTentativas)
                        await Task.Delay(1000); // espera antes do retry
                }
            }
        }

        private async Task<bool> ValidaCotacao(CotacaoDTO cotacao)
        {
            // Em um caso de produção, deve-se utilizar um cache nessa consulta.
            var ativoDaCotacao = await _corretoraContext.Ativos.FindAsync(cotacao.AtivoId);

            if (ativoDaCotacao == null)
            {
                _logger.LogWarning($"Ativo com ID {cotacao.AtivoId} não encontrado.");
                return false;
            }

            if (cotacao.PrecoUnitario <= 0)
            {
                _logger.LogWarning($"Cotação inválida para o ativo {ativoDaCotacao.Nome}: Preço unitário deve ser maior que zero.");
                return false;
            }

            if (cotacao.Identificador == Guid.Empty)
            {
                _logger.LogWarning($"Cotação inválida para o ativo {ativoDaCotacao.Nome}: Identificador não pode ser vazio.");
                return false;
            }

            return ValidaIdentificadorUnico(cotacao.Identificador);
        }
        
        private bool ValidaIdentificadorUnico(Guid identificador)
        {
            // Isso com absoluta certeza não seria a melhor forma de fazer isso. Acredito que o ideal seria ter essa coluna de identificador
            // dentro do banco de dados como uma coluna. 
            // O memory cache é absurdamente mais rápido que o banco de dados mas não é persistente (ou seja, se o worker for reiniciado, o cache será perdido)
            // e além disso, o uso de memória pode ser um problema se o volume de cotações for muito grande.

            if (_cache.TryGetValue(identificador, out _))
            {
                _logger.LogWarning($"Identificador {identificador} já foi processado anteriormente.");
                return false;
            }

            _cache.Set(identificador, true, TimeSpan.FromHours(1));
            return true;
        }
    }
}
