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
                    // En primer lugar se declara una cola
                    channel.QueueDeclare("ColaAT", false, false, false, null);

                    // Se crea un mensaje
                    string message = DateTime.Now.ToString() + " - Mensaje de prueba";
                    byte[] body = Encoding.UTF8.GetBytes(message);

                    // Se publica un mensaje en la cola
                    channel.BasicPublish("", "ColaAT", null, body);
                    Console.WriteLine("Enviado el mensaje: {0}", message);
                }
            }
        }
    }
}
