using StackExchange.Redis;
using System;
using System.Configuration;

namespace cheese_app.Services
{
    public class RedisService
    {
        private static Lazy<ConnectionMultiplexer> _lazyConnection;
        private static readonly object _lock = new object();

        static RedisService()
        {
            InitializeConnection();
        }

        private static void InitializeConnection()
        {
            string connectionString = ConfigurationManager.AppSettings["RedisConnectionString"];

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "localhost:6379";
            }

            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(connectionString);
            });
        }

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return _lazyConnection.Value;
            }
        }

        public static IDatabase GetDatabase()
        {
            return Connection.GetDatabase();
        }

        public static IServer GetServer()
        {
            var endpoint = Connection.GetEndPoints()[0];
            return Connection.GetServer(endpoint);
        }

        public static bool IsConnected()
        {
            return Connection.IsConnected;
        }

        public static string GetConnectionStatus()
        {
            return Connection.GetStatus();
        }
    }
}
