using Mindbox.Kafka;
using Mindbox.Kafka.Abstractions;

namespace DevSchool.Kafka;

public class KafkaConsumerService(
    IAsyncConsumerHostFactory consumerHostFactory,
    IAsyncConsumerMessageProcessor<string> messageProcessor
) : BackgroundService
{
	private readonly AsyncConsumerHostSettings _consumerHostSettings = new()
	{
		InternalChannelSize = 100,
		CommitLogSize = 100,
		WorkerTaskCount = 100,
		CommitterDelay = TimeSpan.FromMilliseconds(1),
		SessionTimeoutMs = 9_000
	};

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var host = consumerHostFactory.CreateHost(
			new Topic(
				() => "demo-topic",
				() => "localhost:29091,localhost:29092,localhost:29093",
				OffsetLossHandling.RedeliverHistoricalData),
			"demo-consumer-group",
			ConsumerFactories.CommonConsumerBuilderFactory(),
			messageProcessor,
			_consumerHostSettings);

		await host.RunAsync(stoppingToken);
	}
}
