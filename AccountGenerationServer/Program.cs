using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DBServer;


namespace AccountGenerationServer
{
    class MainClass
    { 
        public static void Main(string[] args)
        {
            Connection.Open("/Users/jujeong-yeol/Documents/Educations/Database/SourceCode/ipList.txt");
            //while(true)
            //{
            //    string workingCountString = Console.ReadLine();
            //    int workingCount = 0;
            //    if (!int.TryParse(workingCountString, out workingCount))
            //    {
            //        Console.WriteLine("Wrong number");
            //        continue;
            //    }

            //    TestDivided(4, workingCount);
            //}

            Task t1 = Task.Run(() => { TestDivided(4, 245); });
            Task t2 = Task.Run(() => { TestDivided(4, 100); });
           // Task t3 = Task.Run(() => { TestDivided(4, 4096); });
            //Task t4 = Task.Run(() => { TestDivided(4, 8192); });

            t1.Wait();
            //t2.Wait();
            //t3.Wait();
            //t4.Wait();
            //TestDivided(4, 2048);
            //TestDivided(4, 4096);
            //TestDivided(4, 8192);
            //TestDivided(4,1024);

            //TestDivided(1, 1024);
            //TestDivided(4, 1024);

            //TestDivided(1, 1024);
            //TestDivided(4, 1024);
        }

        public static void TestDivided(int count, int requestCount)
        {
            var ipinfos = IPInfoList.FromFile("/Users/jujeong-yeol/Documents/Educations/Database/SourceCode/ipList.txt");
            DBClient[] clients = new DBClient[ipinfos.ips.Count];
            for (int i = 0; i < ipinfos.ips.Count; i++)
            {
                string ip = ipinfos.ips[i].ip;
                int port = ipinfos.ips[i].port;
                DBClient newClient = new DBClient(ip, port,i);
                clients[i] = newClient;
            }

            //while (true)
            //{
            //    Console.WriteLine("Type user id");
            //    var id = Console.ReadLine();

            //    Console.WriteLine("Type newCharacter id");
            //    var chracterId = Console.ReadLine();

            //    int serverCount = 4;
            //    int serverIndex = -1;
            //    using (var sha256 = new SHA256Managed())
            //    {
            //        var hash = new List<byte>(sha256.ComputeHash(Encoding.UTF8.GetBytes(id)));
            //        hash.Add(0);
            //        BigInteger number = new BigInteger(hash.ToArray());
            //        BigInteger index = number % serverCount;
            //        serverIndex = (int)(index);
            //    }

            //    clients[serverIndex].AddCharacter(id, chracterId);  
            //}
            
            Stopwatch outer = new Stopwatch();
            outer.Start();
            //while (requestCount <= 8192)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                for (int i = 0; i < requestCount; i++)
                {
                    var id = GetRandomString();
                    var password = GetRandomString();

                    int serverCount = count;
                    int serverIndex = -1;
                    using (var sha256 = new SHA256Managed())
                    {
                        var hash = new List<byte>(sha256.ComputeHash(Encoding.UTF8.GetBytes(id)));
                        hash.Add(0);
                        BigInteger number = new BigInteger(hash.ToArray());
                        BigInteger index = number % serverCount;
                        serverIndex = (int)(index);
                    }

                    clients[serverIndex].AddUser(id, password);
                }

                sw.Stop();
                Console.WriteLine($"{requestCount} : {sw.ElapsedMilliseconds}");

                requestCount *= 2;
            }
            outer.Stop();
            //Console.WriteLine($"total time : {outer.ElapsedMilliseconds}");
        }

        public static string GetRandomString()
        {
            Random random = new Random();
            int randomNumber = random.Next(int.MinValue, int.MaxValue);
            return randomNumber.ToString();
        }
    }
}