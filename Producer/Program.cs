using RabbitMQ.Client;
using System.Text;

namespace Producer
{
    internal class Program
    {
        // Constante con la IP
        private const string RabbitMqHost = "192.168.0.27";

        // Para el patrón publicación/subscripción
        const string ROUTING_KEY = "alerta.crítica";
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

                    // Claves de enrutado de ejemplo
                    string[] routingKeys = { "alerta.info", "alerta.crítica", "alerta.aviso" };

                    // Enviar mensajes con diferentes claves de enrutado
                    foreach (var routingKey in routingKeys)
                    {
                        string message = $"Mensaje para {routingKey}";
                        byte[] body = Encoding.UTF8.GetBytes(message);

                        // Publicar el mensaje con la clave de enrutado
                        channel.BasicPublish(EXCHANGE, routingKey, null, body);
                        Console.WriteLine($"Enviado: '{message}' con clave de enrutado '{routingKey}'");
                    }
                }
            }
        }
    }
}
