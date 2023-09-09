using System.Diagnostics.Metrics;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQTest
{
    public class RabitLoadTest
    {
        private readonly ConnectionFactory factory;
        public RabitLoadTest(ConnectionFactory connection)
        {
            factory = connection;
        }

        public async Task load(string quename, int number, int worker)
        {
            List<int> keys = Enumerable.Range(1, number).ToList();
            await Parallel.ForEachAsync(keys, new ParallelOptions() { MaxDegreeOfParallelism = worker },
            (item, cancelToken) =>
            {
                return BuildMessage(quename + item.ToString(), worker);
            });

        }

        private async ValueTask BuildMessage(string quename, int worker)
        {
            using (var connection = factory.CreateConnection())
            {
                // Create a channel
                using (var channel = connection.CreateModel())
                {
                    // Declare a queue
                    channel.QueueDeclare(queue: quename, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    int counter = 0;
                    // Create a message
                    while (!Console.KeyAvailable)
                    {
                        Interlocked.Increment(ref counter);
                        var id_key = Guid.NewGuid().ToString();
                        string message = $"Hello, RabbitMQ!_{id_key}";
                        var body = Encoding.UTF8.GetBytes(message);

                        // Publish the message to the queue
                        channel.BasicPublish(exchange: "", routingKey: quename, basicProperties: null, body: body);
                        if (counter % 1000==0)
                            Console.WriteLine("Message sent: {0} queueName : {1}", message, quename);
                    }
                }
            }
        }
    }
}
