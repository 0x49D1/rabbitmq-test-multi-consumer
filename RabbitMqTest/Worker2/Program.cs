using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker2;

class Worker
{
    public static void Main()
    {
        var factory = new ConnectionFactory() {HostName = "localhost", UserName = "user", Password = "password"};
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);
            var queueName = channel.QueueDeclare("test2", durable: true, autoDelete: false, exclusive: false); 
            channel.QueueBind(queue: queueName,
                exchange: "logs",
                routingKey: "");

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);

                int dots = message.Split('.').Length - 1;
                Thread.Sleep(dots * 1000);

                Console.WriteLine(" [x] Done");

                // here channel could also be accessed as ((EventingBasicConsumer)sender).Model
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}