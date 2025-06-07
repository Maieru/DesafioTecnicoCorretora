using Confluent.Kafka;
using Corretora.Model.DTO;
using Corretora.Model.Models;
using System.Text.Json;

namespace Corretora.MonitorCotacoes
{
    /// <summary>
    /// Gera as cota��es de forma peri�dica e as envia para o t�pico Kafka "cotacoes".
    /// >>>> Tem o �nico e exclusivo prop�sito de gerar cota��es para teste <<<<
    /// </summary>
    public class GeradorCotacoesWorker : BackgroundService
    {
        private readonly ILogger<MonitorCotacoesWorker> _logger;
        private readonly ConsumerConfig _configKafka;

        public GeradorCotacoesWorker(ILogger<MonitorCotacoesWorker> logger, ConsumerConfig configuracoesKafka)
        {
            _logger = logger;
            _configKafka = configuracoesKafka;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var producer = new ProducerBuilder<Null, string>(_configKafka).Build();
            var randomGenerator = new Random();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cotacao = new CotacaoDTO
                    {
                        AtivoId = randomGenerator.Next(1, 1000),
                        PrecoUnitario = Convert.ToDecimal(randomGenerator.NextDouble() * 100),
                        Identificador = Guid.NewGuid()
                    };

                    var json = JsonSerializer.Serialize(cotacao);

                    var result = await producer.ProduceAsync("cotacoes", new Message<Null, string>
                    {
                        Value = json
                    });
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, "Erro ao consumir mensagem Kafka.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar cota��o.");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
