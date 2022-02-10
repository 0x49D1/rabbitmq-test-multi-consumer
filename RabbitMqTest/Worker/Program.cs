using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Timers;

class Worker
{
    public static void Main()
    {
        // Test of timer handler
        System.Timers.Timer aTimer = new System.Timers.Timer();
        aTimer.Elapsed += new ElapsedEventHandler((source, e) => Console.Write("Timer Test"));
        aTimer.Interval = 3000;
        // Test timer
        // aTimer.Enabled = true;

        var factory = new ConnectionFactory()
        {
            HostName = "localhost", UserName = "user", Password = "password",
            // DispatchConsumersAsync = true
        };
        var connection = factory.CreateConnection();

        // Add multiple consumers, so that queue can be processed "in parallel"
        for (int i = 1; i < 10; i++)
        {
            var j = i;
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);
            var queueName = channel.QueueDeclare("test1", durable: true, autoDelete: false, exclusive: false); 

            // take 1 message per consumer
            channel.BasicQos(0, 1, false);

            channel.QueueBind(queue: queueName,
                exchange: "logs",
                routingKey: "");

            Console.WriteLine($" [*] Waiting for messages in {j}");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received in {j} -> {message} at {DateTime.Now}");

                // Thread.Sleep(dots * 1000);

                // await Task.Delay(3000);
                Thread.Sleep(10000); // async works too
                
                if (j == 5)
                {
                    // Test special case of returning item to queue: in this case we received the message, but did not process it because of some reason.
                    // QOS is 1, so our consumer is already full. We need to return the message to the queue, so that another consumer can work with it
                    Console.WriteLine($"[-] CANT PROCESS {j} consumer! Error with -> {message}"); 
                    channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, true);
                }
                else
                {
                    Console.WriteLine($" [x] Done {j} -> {message} at {DateTime.Now}");

                    // here channel could also be accessed as ((EventingBasicConsumer)sender).Model
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };
            channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}