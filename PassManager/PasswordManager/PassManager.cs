using System;
using System.Data.SqlClient;
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
    }
}
