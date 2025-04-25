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
                    // Se crea un intercambiador de tipo fanout denominado "Alertas"
                    channel.ExchangeDeclare("Alertas", "fanout");

                    // Se crea una cola temporal
                    var queueName = channel.QueueDeclare().QueueName;

                    // Se enlaza la cola con el intercambiador
                    channel.QueueBind(queueName, "Alertas", "");

                    // Se crea un consumidor
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine("Recibido {0}", message);

                        // Dividimos el mensaje para obtener el número
                        string[] parts = message.Split('-');
                        if (parts.Length >= 3)
                        {
                            // Extraemos el número y lo convertimos a entero
                            string numberPart = parts[2].Trim();
                            if (int.TryParse(numberPart, out int taskNumber))
                            {
                                Console.WriteLine($"Ejecutando tarea número: {taskNumber}");

                                // Simulamos la ejecución de la tarea
                                Console.WriteLine($"Iniciando ejecución de la tarea {taskNumber}...");
                                Thread.Sleep(taskNumber * 1000); // Simulamos el tiempo de ejecución según el número
                                Console.WriteLine($"Tarea {taskNumber} completada exitosamente.");
                            }
                            else
                            {
                                Console.WriteLine("No se pudo extraer un número válido del mensaje.");
                            }
                        }

                        // ACK del mensaje
                        channel.BasicAck(ea.DeliveryTag, false);

                    };
                    // Para que la división de mensajes no se realice al principio
                    channel.BasicQos(0, 1, false);
                    // Se inicia el consumidor
                    channel.BasicConsume(queue: "ColaTareas",
                                      autoAck: false, //valor false = consumidores deben enviar mensaje cuando terminan de procesar un mensaje
                                      consumer: consumer);

                    // Bloquea el consumidor a la espera de recibir mensajes
                    Console.WriteLine("Press enter to exit");
                    Console.ReadLine();
                }
            }
        }
    }
}
