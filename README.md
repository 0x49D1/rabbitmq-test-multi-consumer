### rabbitmq-test-multi-consumer
# THIS IS JUST A SAMPLE!
*Sample* to check RabbitMQ with exchange (to send messages to multiple queues) and multiple consumers in one of the queues, to process the queue faster.    
The project assumes that you have RabbitMQ server running on localhost (docker for example: https://hub.docker.com/_/rabbitmq). Queues are durable. Event based model of message consumption is used.      
Samples were taken from official [RabbitMQ .NET tutorials](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html).    
`NewTask` - project to publish messages via exchange to several queue in RabbitMQ.   
`Worker1` - subscriber 1, uses multi consumer config with QOS 1 message per consumer.   
`Worker2` - subscriber 2, subscribes to second queue, just to use the exchange configuration.
