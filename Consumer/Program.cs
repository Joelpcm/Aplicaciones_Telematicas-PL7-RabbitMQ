using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer
{
    internal class Program
    {
        // Constante con la IP
        private const string RabbitMqHost = "192.168.0.27";

        // Para el patrón publicación/subscripción
        const string BINDING_KEY = "alerta.*";
        const string EXCHANGE = "Alertas";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = RabbitMqHost };
            // Para realizar el envío de información desde el cliente
            // hay que crear una conexión con el servidor
            using (var connection = factory.CreateConnection())
            {
                // A continuación hay que crear un canal,
                // que es una abstracción para enviar y recibir información
                using (var channel = connection.CreateModel())
                {
                    // Se declara un intercambiador de tipo topic denominado "Alertas"
                    channel.ExchangeDeclare(EXCHANGE, "topic");

                    // Se crea una cola temporal
                    var queueName = channel.QueueDeclare();

                    // Se desean recibir todas las alertas
                    channel.QueueBind(queueName, EXCHANGE, BINDING_KEY);

                    Console.WriteLine($"Esperando mensajes con patrón '{BINDING_KEY}'...");

                    // Consumir mensajes
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine($"Recibido: '{message}' con clave de enrutado '{ea.RoutingKey}'");
                    };

                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                    // Bloquea el consumidor a la espera de recibir mensajes
                    Console.WriteLine("Presiona [Enter] para salir.");
                    Console.ReadLine();
                }
            }
        }
    }
}
