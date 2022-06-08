using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DBServer
{
    /// <summary>
    /// communicate with db
    /// </summary>
    public class DBClient
    {
        const string ACCOUNT_TALBE_NAME = "Accounts";
        const string COLUMN_LOGIN_ID_NAME = "loginId";
        const string COLUMN_ID_NAME = "id";
        const string COLUMN_CHARACTER_NAME = "characterId";
        const string CHARACTER_TALBE_NAME = "Characters";
        const string COLUMN_X_NAME = "x";
        const string COLUMN_Y_NAME = "y";
        const string COLUMN_Z_NAME = "z";
        const string COLUMN_NAME_NAME = "name";
        const string COLUMN_PASSWORD_NAME = "password";
        
        public string Ip { get; set; }
        public int Port { get; set; }
        public int connectionIndex { get; set; }
        public DBClient(string ip, int port, int connectionIndex)
        {
            this.Ip = ip;
            this.Port = port;
            this.connectionIndex = connectionIndex;
        }

        public void AddUser(string userId, string password)
        {
            userId = $"\"{userId}\"";
            password = $"\"{password}\"";
            string commandString = $@"
INSERT INTO {ACCOUNT_TALBE_NAME} ({COLUMN_LOGIN_ID_NAME}, {COLUMN_PASSWORD_NAME})
VALUES({userId}, {password});";
            //Console.WriteLine(commandString);
            var connection = Connection.GetConnection(connectionIndex);
            if (null == connection)
                Console.WriteLine("connection is null");

            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            using (var command = new MySqlCommand(commandString, connection))
            {
                try
                {
                    command.ExecuteNonQuery();
                }
                catch(System.Exception e)
                {
                    Console.WriteLine(e.ToString());
                    connection.Close();
                }
            }

            connection.Close();
        }

        public void AddCharacter(string id, string characterName)
        {
            var connection = Connection.GetConnection(connectionIndex);
            var command = new MySqlCommand();
            MySqlTransaction transaction = connection.BeginTransaction();
            command.Connection = connection;
            command.Transaction = transaction;

            string createCharacterString =
                $@"INSERT INTO {CHARACTER_TALBE_NAME} ({COLUMN_NAME_NAME}) VALUES ({characterName});";

            string setCharacterIdString =
                $@"UPDATE {ACCOUNT_TALBE_NAME} SET {COLUMN_CHARACTER_NAME} = LAST_INSERT_ID() WHERE loginId = {id};";

            command.CommandText = createCharacterString;
            var result = command.ExecuteNonQuery();
            if (0 == result)
            {
                transaction.Rollback();
                connection.Close();
                return;
            }

            command.CommandText = setCharacterIdString;

            result = command.ExecuteNonQuery();
            if (0 == result)
            {
                transaction.Rollback();
                connection.Close();
                return;
            }

            transaction.Commit();
            connection.Close();
        }

        public void ChangePosition(ulong id, float x, float y , float z)
        {
            var connection = Connection.GetConnection(connectionIndex);
            string commandString = $@"UPDATE {CHARACTER_TALBE_NAME}
SET
{COLUMN_X_NAME} = {x},
{COLUMN_Y_NAME} = {y},
{COLUMN_Z_NAME} = {z}
WHERE
{COLUMN_ID_NAME} = {id};
";

            using (var command = new MySqlCommand(commandString, connection))
            {
                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        public byte[] GetSha512(string plainText)
        {
            var sha = new System.Security.Cryptography.SHA512Managed();
            byte[] data = sha.ComputeHash(Encoding.UTF8.GetBytes(plainText));
            return data;
        }

        public ulong GetCharacterId(string userId)
        {
            userId = $"\"{userId}\"";
            var connection = Connection.GetConnection(connectionIndex);
            string commandString =
                $@"SELECT * FROM {ACCOUNT_TALBE_NAME} WHERE loginId = {userId};";

            //Console.WriteLine($"commandString : {commandString}");

            using (var command = new MySqlCommand(commandString, connection))
            {
                //Console.WriteLine("created command");
                using (var reader = command.ExecuteReader())
                {
                    //Console.WriteLine("read async completed");
                    while(reader.Read())
                    {
                        var characterid = reader[$"{COLUMN_CHARACTER_NAME}"];
                        if (characterid is ulong)
                        {
                            connection.Close();
                            return (ulong)characterid;
                        }
                        else
                        {
                            //Console.WriteLine("type of characterId : " + characterid.GetType().ToString());
                            connection.Close();
                        }
                    }
                }
            }

            connection.Close();
            return 0;
        }

        public MySqlConnection GetNewConnection()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var newConnection = new MySqlConnection($@"Server={Ip};
            Port={Port};
            UserID=root;
            Password=12345678;
            Database=Metaverse;
            Pooling=true;
            Min Pool Size = 5;
            Max Pool Size = 100");
            //var newConnection = new MySqlConnection($@"Server={Ip};
            //Port={Port};
            //UserID=root;
            //Password=12345678;
            //Database=Metaverse");

            try
            {
                newConnection.Open();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            sw.Stop();
            //Console.WriteLine($"open elapsed mili : {sw.ElapsedMilliseconds}");
            return newConnection;
        }
    }
}
