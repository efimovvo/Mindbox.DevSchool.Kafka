using Confluent.Kafka;
using Confluent.Kafka.Admin;

const string bootstrapServers = "localhost:29091,localhost:29092,localhost:29093";
const string topicName = "demo-topic";

using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build();


// Получить сообщение с офсетом 15
var config1 = new ConsumerConfig
{
    BootstrapServers = bootstrapServers,
    GroupId = "consumer-group",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

const int patition = 0;
const long offset = 15;

using var consumer1 = new ConsumerBuilder<Ignore, string>(config1).Build();

var topicPartition = new TopicPartition(topicName, patition);
consumer1.Assign(new[] { topicPartition });
consumer1.Consume();

consumer1.Seek(new TopicPartitionOffset(topicPartition, offset));

var message1 = consumer1.Consume();

if (message1 != null)
    Console.WriteLine($"{message1.Message.Value}");


// Переместить оффсет на 10 и прочитать заново
var config2 = new ConsumerConfig
{
    BootstrapServers = bootstrapServers,
    GroupId = "consumer-group",
    AutoOffsetReset = AutoOffsetReset.Earliest,
    EnableAutoCommit = false
};

var metadata = adminClient.GetMetadata(topicName, TimeSpan.FromSeconds(10));
var partitions = metadata.Topics[0].Partitions;

var topicPartitionOffsets = new List<TopicPartitionOffset>();

foreach (var partition in partitions)
    topicPartitionOffsets.Add(new TopicPartitionOffset(new TopicPartition(topicName, partition.PartitionId), new Offset(10)));

var groupPartitionOffsets = new List<ConsumerGroupTopicPartitionOffsets>
{
    new ConsumerGroupTopicPartitionOffsets(
        config2.GroupId,
        topicPartitionOffsets
    )
};

await adminClient.AlterConsumerGroupOffsetsAsync(groupPartitionOffsets);

using var consumer2 = new ConsumerBuilder<Ignore, string>(config2).Build();

consumer2.Subscribe(topicName);

while (true)
{
    var message2 = consumer2.Consume(TimeSpan.FromSeconds(1));

    if (message2 != null && message2.Offset > 10)
        Console.WriteLine($"Partition: {message2.Partition}, Offset: {message2.Offset}, Value: {message2.Message.Value}");
}
