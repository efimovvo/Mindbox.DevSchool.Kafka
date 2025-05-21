using Confluent.Kafka;

namespace DevSchool.Kafka;

public class KafkaConsumerService : BackgroundService
{
	private readonly ILogger<KafkaConsumerService> _logger;
	private readonly IConsumer<string, string> _consumer;

	public KafkaConsumerService(ILogger<KafkaConsumerService> logger, ConsumerConfig config)
	{
		_logger = logger;
		_consumer = new ConsumerBuilder<string, string>(config).Build();
		_consumer.Subscribe("demo-topic");
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		return Task.Run(() =>
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var result = _consumer.Consume(stoppingToken);
					_logger.LogInformation("Consumed: {Message}", result.Message.Value);
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error while consuming message");
				}
			}
		}, stoppingToken);
	}

	public override void Dispose()
	{
		_consumer.Close();
		_consumer.Dispose();
		base.Dispose();
	}
}