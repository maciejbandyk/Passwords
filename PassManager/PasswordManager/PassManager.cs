using System;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PassManager
{
    public class PassManger
    {
        //method receives sql commands and send them to Azure
        public void AzureConnect(string[] commands)
        {
            try
            {
                SqlConnectionStringBuilder build = new SqlConnectionStringBuilder();
                build.DataSource = "password.database.windows.net";
                build.UserID = "Admin1";
                build.Password = "Pass123!";
                build.InitialCatalog = "PassManager";

                using (SqlConnection connection = new SqlConnection(build.ConnectionString))
                {
                    //Console.WriteLine("Connection estabilished");

                    connection.Open();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(commands[0]);  //send sql command from commands arr to azure (every line is new append)
                    String sql = sb.ToString();

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        //command received
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //Console.WriteLine(reader.GetString(1) + "|" + reader.GetString(2)); //print results
                            }
                        }
                    }
                }
            }
            catch (SqlException sE)
            {
                Console.WriteLine(sE.ToString());
            }
        }
        //method for encrypt password
        public static string Encrypt(string key, string text)
        {
            ///create arrays for holding bytes
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);  ///convert key to bytes
                aes.IV = iv;
                ///transformation interface gets key and iv values
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream()) ///write stream to memory
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) ///connect stream
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream)) ///saves stream
                        {
                            streamWriter.Write(text);
                        }
                        ///put bytes into array
                        array = memoryStream.ToArray();
                    }
                }

            }
            ///convert array into string
            return Convert.ToBase64String(array);
        }
        ///method for decrypt password
        public static string Decrypt(string key, string text)
        {
            ///create array for bytes
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(text);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key); ///convert bytes to stream
                aes.IV = iv;

                ///transformation interface gets key and iv values
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer)) ///write stream to memory
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)) ///connect stream
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream)) ///reads stream
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
