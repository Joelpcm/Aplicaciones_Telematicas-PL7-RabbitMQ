using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Consumer
{
    internal class Program
    {
        // Constante con la IP
        private const string RabbitMqHost = "192.168.0.27";
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
                    //En primer lugar se declara una cola
                    channel.QueueDeclare("ColaAT", false, false, false, null);

                    // Se crea un consumidor
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        Console.WriteLine("Recibido {0}", message);
                    };
                    channel.BasicConsume(queue: "ColaAT",
                                      autoAck: true,
                                      consumer: consumer);

                    // Bloquea el consumidor a la espera de recibir mensajes
                    Console.WriteLine("Press enter to exit");
                    Console.ReadLine();
                }
            }
        }
    }
}
