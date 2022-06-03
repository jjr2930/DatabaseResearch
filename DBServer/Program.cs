using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace DBServer
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            IPInfoList iPInfoList = new IPInfoList();
            iPInfoList.AddIp("localhost", 9000);
            iPInfoList.AddIp("localhost", 9001);
            iPInfoList.AddIp("localhost", 9002);
            iPInfoList.AddIp("localhost", 9003);
            iPInfoList.AddIp("localhost", 9004);
            iPInfoList.AddIp("localhost", 9005);
            iPInfoList.AddIp("localhost", 9006);
            iPInfoList.AddIp("localhost", 9007);

            iPInfoList.Save("ipList.txt");

            return;
            Console.WriteLine("Hello World!");
            DBClient dBClient = new DBClient("localhost", 3306);
            //add user
            //buildrandombytes
            int taskSize = 1;
            var tasks = new Task[taskSize];
            for (int i = 0; i < taskSize; i++)
            {
                tasks[i] = Task.Run(
                    () =>
                    {
                        //Stopwatch innerSW = new Stopwatch();
                        //innerSW.Start();
                        string randomUserId = GetRandomString();
                        dBClient.AddUser(randomUserId);
                        //Console.WriteLine($"create randomuser completed {randomUserId}");

                        string randomCharacterId = GetRandomString();
                        dBClient.AddCharacter(randomUserId, randomCharacterId);
                        //Console.WriteLine($"create random character completed {randomUserId}, {randomCharacterId}");

                        ulong characterId = dBClient.GetCharacterId(randomUserId);
                        //Console.WriteLine($"get character id completed {characterId}");

                        var movingStartTime = DateTime.Now;
                        var lastMovingTime = DateTime.Now;
                        var _10SecondsTick = TimeSpan.TicksPerSecond * 10f;
                        var _5SecondsTick = TimeSpan.TicksPerSecond * 5f;
                        //Console.WriteLine($"{innerSW.ElapsedTicks} ticks");
                        //while (DateTime.Now.Ticks - movingStartTime.Ticks < _10SecondsTick)
                        //{
                        //    if (DateTime.Now.Ticks - lastMovingTime.Ticks >= _5SecondsTick)
                        //    {
                        //        dBClient.ChangePosition(characterId, 3, 4, 5);
                        //        lastMovingTime = DateTime.Now;
                        //    }
                        //}
                    });
            }

            var sw = new Stopwatch();
            sw.Reset();
            sw.Start();
            for (int i = 0; i < taskSize; i++)
            {
                tasks[i].Wait();
            }
            sw.Stop();
            Console.WriteLine("elapssed time : " + sw.ElapsedMilliseconds);
        }

        public static string GetRandomString()
        {
            Random random = new Random();
            int randomNumber = random.Next(1,99999);
            return randomNumber.ToString();
        }
    }
}
