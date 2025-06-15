using Mindbox.Kafka;
using Mindbox.Kafka.Abstractions;

namespace DevSchool.Kafka;

public sealed class MyMessageProcessor(ILogger<MessageProcessor> logger) : IAsyncConsumerMessageProcessor<string>
{
	public string Deserialize(string rawMessage)
		=> rawMessage;

	public Task<bool> DiscardIfNeedAsync(ConsumeResult<string> message, CancellationToken token)
		=> Task.FromResult(false);

	public async Task ProcessAsync(ConsumeResult<string> message, CancellationToken token)
	{
		// Couldn't be changed, it's constant
		var messageProcessingTime = TimeSpan.FromMilliseconds(1);

		await Task.Delay(messageProcessingTime, token);
		logger.LogInformation("Consumed: {message}", message);
	}
}
