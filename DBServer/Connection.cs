using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace DBServer
{
    public static class Connection
    {
        static List<MySqlConnection> connections = null;

        public static void Open(string infoPath)
        {
            connections = new List<MySqlConnection>();

            var ipinfos = IPInfoList.FromFile(infoPath);
            
            for (int i = 0; i < ipinfos.ips.Count; i++)
            {
                var ip = ipinfos.ips[i].ip;
                var port = ipinfos.ips[i].port;

                string connectionString = $@"Server={ip};
            Port={port};
            UserID=root;
            Password=12345678;
            Database=Metaverse;
            Pooling=true;
            Min Pool Size = 5;
            Max Pool Size = 100";

                var newConnection = new MySqlConnection(connectionString);
                newConnection.Open();
                connections.Add(newConnection);
            }
        }

        public static MySqlConnection GetConnection(int index)
        {
            var connection = connections[index];
            //connection.Open();
            return connection;
        }
    }
}
