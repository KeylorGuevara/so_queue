using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.ServiceBus;
using System.Data.SqlClient;


namespace azure_connection_sql
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://service-bus-queue.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=2JoqjrWyYZMOnHGtFBhtPDfGhauso3y0krQGqA0PpSw=";
        const string QueueName = "queue_so";
        static IQueueClient queueClient;

        static List<List<string>> conciertos = new List<List<string>>();
        static List<string> concierto1 = new List<string>() { "Circo Soledad", "Ricardo Saprissa", "Ricardo Arjona", "225" };
        static List<string> concierto2 = new List<string>() { "Prismatic World Tour", "Parque Viva", "Katy Perry", "Golden Circle" };
        static List<string> concierto3 = new List<string>() { "La última vez", "Teatro Melico Salazar", "Joaquín Sabina", "Lunada" };
        static List<string> concierto4 = new List<string>() { "The Killers World Tour", "Parque Viva", "The Killers", "232" };
        static List<string> concierto5 = new List<string>() { "Tour Prometo", "Palacio de los Deportes", "Pablo Alborán", "225" };
        static List<string> concierto6 = new List<string>() { "Joanne World Tour", "Ricardo Saprissa", "Lady Gaga", "1" };
        

        static List<int> tiquetes_comprados = new List<int>();

        static void Main(string[] args)
        {
            conciertos.Add(concierto1);
            conciertos.Add(concierto2);
            conciertos.Add(concierto3);
            conciertos.Add(concierto4);
            conciertos.Add(concierto5);
            conciertos.Add(concierto6);

            MainAsync().GetAwaiter().GetResult();
            insercion_sql();
        }

        static async Task MainAsync()
        {
            queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            Console.WriteLine("======================================================");
            Console.WriteLine("Press ENTER key to exit after receiving all the messages.");
            Console.WriteLine("======================================================");

            // Register QueueClient's MessageHandler and receive messages in a loop
            RegisterOnMessageHandlerAndReceiveMessages();

            Console.ReadKey();

            await queueClient.CloseAsync();
        }

        static void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the MessageHandler Options in terms of exception handling, number of concurrent messages to deliver etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of Concurrent calls to the callback `ProcessMessagesAsync`, set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false
            };

            // Register the function that will process messages
            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            try
            {
                // Process the message
                Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");
                string tiquete_escogido = Encoding.UTF8.GetString(message.Body);
                int numero_tiquete = Int32.Parse(tiquete_escogido);
                tiquetes_comprados.Add(numero_tiquete);
               
                await queueClient.CompleteAsync(message.SystemProperties.LockToken);
                

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            //Console.WriteLine($"Message id: {message.MessageId}");

        }
            

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

        static void insercion_sql()
        {
            foreach (int tik in tiquetes_comprados)
            {
                int tiquete = tik;
                try
                {
                    var cb = new SqlConnectionStringBuilder();
                    cb.DataSource = "server-queue.database.windows.net";
                    cb.UserID = "usuario-queue";
                    cb.Password = "$contrasena-3";
                    cb.InitialCatalog = "so_project-azure";
                    using (var connection = new SqlConnection(cb.ConnectionString))
                    {
                        connection.Open();
                        Submit_Tsql_NonQuery(connection, "Inserts",
                           Build_3_Tsql_Inserts(tiquete));
                    }
                }
                catch (SqlException e)
                {
                    Console.WriteLine(e.ToString());
                }
                Console.WriteLine("View the report output here, then press any key to end the program...");
              //  Console.ReadKey();

            }
        }

        static string Build_3_Tsql_Inserts(int tiquete)
        {
            //Console.WriteLine("Llego a la funcion especifica para insertar");
            
            List<string> concierto_escogido = conciertos[tiquete];
            string nombre_concierto = concierto_escogido[0];
            string lugar = concierto_escogido[1];
            string artista = concierto_escogido[2];
            string numero_asiento = concierto_escogido[3];
            
            var string_conexion = string.Format(@" INSERT INTO concierto (nombre_concierto, lugar,artista,numero_asiento) VALUES ('{0}','{1}','{2}','{3}');", nombre_concierto, lugar, artista, numero_asiento);
            return string_conexion;

            /*return @" INSERT INTO concierto (nombre_concierto, lugar,artista,numero_asiento) 
                VALUES
                    ('Circo Soledad','Ricardo Saprissa','Ricardo Arjona','225'),
                    ('Prismatic World Tour','Parque Viva','Katy Perry','Golden Circle');
            ";*/


        }


  
        static void Submit_Tsql_NonQuery(
         SqlConnection connection,
         string tsqlPurpose,
         string tsqlSourceCode,
         string parameterName = null,
         string parameterValue = null
         )
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine("T-SQL to {0}...", tsqlPurpose);

            using (var command = new SqlCommand(tsqlSourceCode, connection))
            {
                if (parameterName != null)
                {
                    command.Parameters.AddWithValue(  // Or, use SqlParameter class.
                       parameterName,
                       parameterValue);
                }
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine(rowsAffected + " = rows affected.");
            }
        }
    } // EndOfClass

}
