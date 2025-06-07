using Confluent.Kafka;
using Corretora.Bussiness.Database;
using Corretora.MonitorCotacoes;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// O certo seria colocar em um vault key, mas como isso � s� uma atividade, vou deixar assim.
var sqlServerConnectionString = "data source=.\\SQLEXPRESS;initial catalog=corretora;trusted_connection=true;TrustServerCertificate=True";

var configuracoesKafka = new ConsumerConfig
{
    BootstrapServers = "localhost:9092", // Acredito que isso deva ficar em algum tipo de vault
                                         // ou gerenciador de configura��es (como o Azure App Configuration)
                                         // N�o coloquei aqui por que n�o � o foco (eu acho).
    GroupId = "cotacoes-consumer-group",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

builder.Services.AddMemoryCache();
builder.Services.AddHostedService<GeradorCotacoesWorker>();
builder.Services.AddHostedService<MonitorCotacoesWorker>();
builder.Services.AddDbContext<CorretoraContext>(options => options.UseSqlServer(sqlServerConnectionString), ServiceLifetime.Singleton);
builder.Services.AddSingleton(configuracoesKafka);

var host = builder.Build();
host.Run();