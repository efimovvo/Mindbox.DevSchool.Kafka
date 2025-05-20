using Confluent.Kafka;
using DevSchool.Kafka;
using Mindbox.Kafka;
using Mindbox.Kafka.Template;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IAsyncConsumerHostFactory, AsyncConsumerHostFactory>();

builder.Services.AddKafkaProducerDependencies(
	builder.Configuration,
	provider => (_, exception, _) =>
	{
		var errorLogger = provider.GetRequiredService<ILogger<ProducerFactory>>();
		errorLogger.LogError(exception, "Error occured");
	});

builder.Services.AddHostedService<KafkaProducerService>();
builder.Services.AddHostedService<KafkaConsumerService>();

builder.Services.AddSingleton(new ProducerConfig
{
	BootstrapServers = "localhost:29091"
});

builder.Services.AddSingleton(new ConsumerConfig
{
	BootstrapServers = "localhost:29091",
	GroupId = "demo-consumer-group",
	AutoOffsetReset = AutoOffsetReset.Earliest,
	EnableAutoCommit = true
});

var app = builder.Build();
app.MapGet("/", () => "Kafka Producer and Consumer running...");
app.Run();

/*var producer = Producers.Reliable(
	"producer-key",
	() => producerFactory,
	kafkaTopicsManagerFactory,
	GetTopicSpecificationParameters(topicSystemName),
	new ProducerConfigParameters
	{
		MessageTimeout = KafkaProducerDefaults.ProduceTimeout,
		Linger = TimeSpan.FromMilliseconds(50),
		BatchSize = KafkaProducerDefaults.MessageMaxBytesForLargeMessages,
		MessageMaxBytes = KafkaProducerDefaults.MessageMaxBytesForLargeMessages,
		SocketNagleDisable = true,
		TopicMetadataPropagationMaxTimeout = TimeSpan.FromMilliseconds(700),
		CompressionType = CompressionType.Gzip
	},
	enableIdempotence: false,
	delayBeforeReproduce: TimeSpan.FromMilliseconds(1200));*/