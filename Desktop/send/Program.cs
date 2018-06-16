using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace cliente_escritorio
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://service-bus-queue.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=2JoqjrWyYZMOnHGtFBhtPDfGhauso3y0krQGqA0PpSw=";
        const string QueueName = "queue_so";
        static IQueueClient queueClient;

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            const int numberOfMessages = 10;
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            Console.WriteLine("======================================================");
            Console.WriteLine("Press ENTER key to exit after sending all the messages.");
            Console.WriteLine("======================================================");

            // Send messages.
            await SendMessagesAsync(numberOfMessages);

            Console.ReadKey();

            await queueClient.CloseAsync();
        }

        static async Task SendMessagesAsync(int numberOfMessagesToSend)
        {
            try
            {
                for (var i = 0; i < numberOfMessagesToSend; i++)
                {
                    int purchased_ticket = compra_tiquete();
                    
                    // Create a new message to send to the queue.
                    string messageBody = purchased_ticket.ToString();
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));

                    // Write the body of the message to the console.
                    Console.WriteLine($"Sending message: {messageBody}");

                    // Send the message to the queue.
                    await queueClient.SendAsync(message);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }

        static int compra_tiquete()
        {
            Random rnd = new Random();
            //Cada uno de los conciertos disponibles
            Console.WriteLine("Hola llegue a prueba");
           
            //Selecciono uno de los conciertos
            int tiquete = rnd.Next(0, 5);
            //List<string> concierto_seleccionado = conciertos[tiquete];
            return tiquete;


        }


    }
}

