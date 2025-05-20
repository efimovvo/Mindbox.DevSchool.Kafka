using Confluent.Kafka;

namespace DevSchool.Kafka;

public class KafkaProducerService : BackgroundService
{
	private readonly ILogger<KafkaProducerService> _logger;
	private readonly IProducer<Null, string> _producer;

	public KafkaProducerService(ILogger<KafkaProducerService> logger, ProducerConfig config)
	{
		_logger = logger;
		_producer = new ProducerBuilder<Null, string>(config).Build();
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		int counter = 0;
		while (!stoppingToken.IsCancellationRequested)
		{
			var message = $"Message #{++counter} at {DateTime.UtcNow:O}";
			await _producer.ProduceAsync("demo-topic", new Message<Null, string> { Value = message }, stoppingToken);
			_logger.LogInformation("Produced: {Message}", message);

			await Task.Delay(1000, stoppingToken);
		}
	}

	public override void Dispose()
	{
		_producer.Flush(TimeSpan.FromSeconds(5));
		_producer.Dispose();
		base.Dispose();
	}
}