using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using LevelUpCSharp.Networking;
using LevelUpCSharp.Production;
using LevelUpCSharp.Products;
using LevelUpCSharp.Reflection;
using Newtonsoft.Json;

namespace LevelUpCSharp.Server
{
    class Program
    {
        private static readonly IEnumerable<Vendor> _vendors = new[] { new Vendor("Slimak") };

        private static IDictionary<string, Route> _handlers;

        static void Main(string[] args)
        {
	        _handlers = ScanForHandlers(Assembly.GetExecutingAssembly());

            var server = BuildServer();

            // Start listening for client requests.
            server.Start();

            var listener = new Task(() => Listen(server), TaskCreationOptions.LongRunning);

            listener.Start();

            Console.ReadKey(true);
            Console.WriteLine("Killing server...");
            server.Stop();
            Console.WriteLine("Killed");
        }

        private static void Listen(TcpListener server)
        {
            // Enter the listening loop.
            while (true)
            {
                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                TcpClient client = server.AcceptTcpClient();

                Console.WriteLine("Connected!");
                ProcessRequest(client);
                client.Close();
                Console.WriteLine("Closed!");
            }
        }

        private static Sandwich[] GetSandwiches()
        {
            return new ProductionHandler(_vendors).Sandwiches().ToArray();
        }

        #region networking
        private static TcpListener BuildServer()
        {
	        var server = new TcpListener(IPAddress.Any, 13000);
	        return server;
        }

        private static void ProcessRequest(TcpClient client)
        {
            using (NetworkStream stream = client.GetStream())
            {

                var cmd = ReadCommand(stream);

                Console.WriteLine("Received: {0}", cmd);

                var sandwiches = GetSandwiches();

                SendResponse(sandwiches, stream);

                Console.WriteLine("Responsed");
            }

        }
        private static string ReadCommand(NetworkStream stream)
        {
	        Byte[] bytes = new Byte[256];
	        string data;

	        var i = stream.Read(bytes, 0, bytes.Length);

	        // Translate data bytes to a ASCII string.
	        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
	        return data;
        }

        public static void SendResponse<TValue>(TValue value, Stream s)
        {
            using (StreamWriter writer = new StreamWriter(s))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
                {
                    JsonSerializer ser = new JsonSerializer();
                    ser.Serialize(jsonWriter, value);
                    jsonWriter.Flush();
                }
            }
        }

        #endregion
        
        #region reflection
        private static object InvokeWorker(Type handler, string method, object instance)
        {
	        return handler.GetMethod(method).Invoke(instance, null);
        }

        private static object ConstructHandler(Type handler)
        {
	        return Activator.CreateInstance(handler, _vendors);
        }

        private static IDictionary<string, Route> ScanForHandlers(Assembly assembly)
        {
            var handlerAttribute = typeof(CtrlAttribute);

            return Reflector.FindByAttributes(assembly, handlerAttribute)
                .ToDictionary(
                    typeInfo => ((CtrlAttribute) typeInfo.GetCustomAttribute(handlerAttribute)).Name,
                    BuildMethodMap);
        }

        private static Route BuildMethodMap(TypeInfo ctrl)
        {
            var workerAttribute = typeof(WorkerAttribute);

            var methods = Reflector.FindByAttributes(ctrl, workerAttribute)
                .ToDictionary(
                    m => ((WorkerAttribute) m.GetCustomAttribute(workerAttribute)).Name,
                    m => m.Name);
            return new Route(ctrl, methods);
        }

        #endregion
    }
}
