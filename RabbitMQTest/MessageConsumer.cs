using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Collections;

namespace RabbitMQTest
{
    public class MessageConsumer
    {
        private readonly ConnectionFactory factory;
        public MessageConsumer(ConnectionFactory connection)
        {
            factory = connection;
        }


        public async Task main( string queue,int number,int worker)
        {
            List<int> keys = Enumerable.Range(1, number).ToList();
            await Parallel.ForEachAsync(keys, new ParallelOptions() { MaxDegreeOfParallelism = worker },
            (item, cancelToken) =>
            {
                return Cousomers(queue + item.ToString(), worker);
            });
            // Create a connection to RabbitMQ

        }

        private async ValueTask Cousomers(string queue,int worker)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {

                    
                    channel.QueueDeclare(queue: queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
                    while (!Console.KeyAvailable)
                    {
                        var consumer1 = new EventingBasicConsumer(channel);
                        consumer1.Received += (model, ea) =>
                        {
                            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                            if (worker <= 10)
                                Console.WriteLine("Queue 1 - Message received: {0}", message);
                        };

                        channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer1);
                    }
                }

                Console.WriteLine("Press any key to exit...");
                Console.ReadLine();
            }
        }
    }
}
