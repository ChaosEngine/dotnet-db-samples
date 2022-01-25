using System;
using Oracle.ManagedDataAccess.Client;

namespace ODP.NET_Core_Autonomous
{
    class Program
    {
        static void Main(string[] args)
        {
            //Demo: ODP.NET Core application that connects to Oracle Autonomous DB without a wallet

            //Enter user id and password, such as ADMIN user	
            string conString = "User Id=<USER ID>;Password=<PASSWORD>;" +

            //Enter net service name or the full connect descriptor for data source value
            "Data Source=<NET SERVICE NAME>;";

            using (OracleConnection con = new OracleConnection(conString))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        con.Open();

                        Console.WriteLine("Successfully connected to Oracle Autonomous Database");

                        //Retrieve database version info
                        cmd.CommandText = "SELECT BANNER FROM V$VERSION";
                        OracleDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        Console.WriteLine("Connected to " + reader.GetString(0));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    Console.ReadLine();
                }
            }
        }
    }
}

/* Copyright (c) 2022, Oracle and/or its affiliates. All rights reserved. */

/******************************************************************************
 *
 * You may not use the identified files except in compliance with The MIT
 * License (the "License.")
 *
 * You may obtain a copy of the License at
 * https://github.com/oracle/Oracle.NET/blob/master/LICENSE
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 *****************************************************************************/
