using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using DBServer;


namespace AccountGenerationServer
{
    class MainClass
    { 
        public static void Main(string[] args)
        {
            var ipinfos = IPInfoList.FromFile("/Users/jujeong-yeol/Documents/Educations/Database/SourceCode/ipList.txt");
            DBClient[] clients = new DBClient[ipinfos.ips.Count];
            for (int i = 0; i < ipinfos.ips.Count; i++)
            {
                string ip = ipinfos.ips[i].ip;
                int port = ipinfos.ips[i].port;
                DBClient newClient = new DBClient(ip, port);
                clients[i] = newClient;
            }

            while(true)
            {
                Console.WriteLine("Type user id");
                var id = Console.ReadLine();

                Console.WriteLine("type your password");
                var password = Console.ReadLine();

                int serverCount = 4;
                int serverIndex = -1;
                using(var sha512 = new SHA512Managed())
                {
                    var hash = new List<byte>(sha512.ComputeHash(Encoding.UTF8.GetBytes(id)));
                    hash.Add(0);
                    BigInteger number = new BigInteger(hash.ToArray());
                    BigInteger index = number % serverCount;
                    serverIndex = (int)(index);
                }

                clients[serverIndex].AddUser(id);
            }
        }
    }
}
