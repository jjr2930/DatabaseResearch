using System;
namespace DBServer
{
    public class Server
    {
        string ip = "localhost";
        int port = 9000;
        DBClient client;
        public Server()
        {
            client = new DBClient(ip, port);
        }
    }
}
