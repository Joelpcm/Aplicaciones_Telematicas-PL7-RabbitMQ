using RabbitMQ.Client;
using System.Text;

namespace Producer
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
                    // Los mensajes se establecen como persistentes
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    // En primer lugar se declara una cola
                    bool durable = true;
                    channel.QueueDeclare("ColaTareas", durable, false, false, null);

                    // Para hacer 10 tareas de 1 segundo y 1 de 10 segundos
                    int Number_1 = 1;
                    int Number_2 = 10;

                    string message;
                    byte[] body;

                    for (int i = 0; i < 10; i++)
                    {    
                        // Se crea un mensaje que incluye el número aleatorio
                        message = $"{DateTime.Now} - Mensaje de prueba - {Number_1.ToString()}";
                        body = Encoding.UTF8.GetBytes(message);
                        // Se publica un mensaje en la cola
                        channel.BasicPublish("", "ColaTareas", properties, body);
                        Console.WriteLine("Enviado el mensaje: {0}", message);
                    }
                    // Se crea un mensaje que incluye el número
                    message = $"{DateTime.Now} - Mensaje de prueba - {Number_2.ToString()}";
                    body = Encoding.UTF8.GetBytes(message);

                    // Se publica un mensaje en la cola
                    channel.BasicPublish("", "ColaTareas", properties, body);
                    Console.WriteLine("Enviado el mensaje: {0}", message);
                }
            }
        }
    }
}
