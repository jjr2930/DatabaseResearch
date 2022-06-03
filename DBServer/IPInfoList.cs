using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DBServer
{
    public class IPInfoList
    {
        public class IPInfo
        {
            public string ip;
            public int port;
            public IPInfo(string ip, int port)
            {
                this.ip = ip;
                this.port = port;
            }
        }

        public List<IPInfo> ips = new List<IPInfo>();

        public static IPInfoList FromFile(string path)
        {
            if (!File.Exists(path))
                throw new System.InvalidOperationException("File is not exist");

            string json = File.ReadAllText(path);
            var ipList = JsonConvert.DeserializeObject<IPInfoList>(json);
            return ipList;
        }

        public void AddIp(string ip, int port)
        {
            ips.Add(new IPInfo(ip, port));
        }

        public void Save(string path)
        {
            if(File.Exists(path))
                File.Delete(path);

            string json = JsonConvert.SerializeObject(this);
            File.WriteAllText(path, json, System.Text.Encoding.UTF8);
        }
    }
}
