
//TEST
using MktSrvcAPI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LivePriceServer
{
    class Conn
    {
        public static MySqlConnection _conn;
        public static DepthOfBkClient _dobkUnderClient; //  client for equity quote
        public static DepthOfBkClient _DepthofBkClient;//Client for quotes
        public static Dictionary<string, List<float>> Dict;
        public static Dictionary<string, List<int>> Dict2;
        public static void connDB()
        {
            if (_conn == null)
            {
                try
                {
                    string _connString = @" SERVER=10.0.7.162;DATABASE=options;UID=webuser;PASSWORD=$calp;default command timeout=5000";
                    _conn = new MySqlConnection(_connString);
                    _conn.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Dict = loadInfo();
            //Dict2 = loadInfo2();
            EquityAPI();
            QuotesAPI();
            Console.WriteLine("Finish Conn...");
        }// Connect to DataBase


        public static void QuotesAPI()
        {
            if (_DepthofBkClient == null)
            {
                //try
                //{
                _DepthofBkClient = new DepthOfBkClient();
                if (!_DepthofBkClient.IsConnected())
                {
                    _DepthofBkClient.Connect("10.10.20.100", 13001);
                    while (!_DepthofBkClient.IsConnected())
                    {
                        Thread.Sleep(100);
                    }
                }
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex);
                //}
            }
        }


        public static void EquityAPI()
        {
            if (_dobkUnderClient == null)
            {
                //try
                //{
                _dobkUnderClient = new DepthOfBkClient();
                if (!_dobkUnderClient.IsConnected())
                {
                    _dobkUnderClient.Connect("10.10.20.100", 12001);
                    while (!_dobkUnderClient.IsConnected())
                    {
                        Thread.Sleep(100);
                    }
                }

                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}
            }
        }//Equity data


        public static Dictionary<string, List<float>> loadInfo()
        {
            Dictionary<string, List<float>> _instrlist = new Dictionary<string, List<float>>();
            string _cmdString = string.Format(@"select * from instrument");
            using (MySqlCommand command = new MySqlCommand(_cmdString, _conn))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string id = reader[0].ToString() + reader[1].ToString() + (reader[2].ToString() == "C" ? "CALL" : "PUT");
                        float strike = float.Parse(reader[3].ToString());

                        if (_instrlist.ContainsKey(id))
                        {
                            _instrlist[id].Add(strike);
                        }
                        else
                        {
                            List<float> strikeList = new List<float>();
                            strikeList.Add(strike);
                            _instrlist.Add(id, strikeList);
                        }
                    }
                }
            }

            return _instrlist;
        }

        public static Dictionary<string, List<int>> loadInfo2()
        {
            Dictionary<string, List<int>> _instrlist = new Dictionary<string, List<int>>();
            string _cmdString = string.Format(@"select * from instrument");
            using (MySqlCommand command = new MySqlCommand(_cmdString, _conn))
            {
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string id = reader[0].ToString() + reader[3].ToString() + (reader[2].ToString() == "C" ? "CALL" : "PUT");
                        int exp = int.Parse(reader[1].ToString());

                        if (_instrlist.ContainsKey(id))
                        {
                            _instrlist[id].Add(exp);
                        }
                        else
                        {
                            List<int> expList = new List<int>();
                            expList.Add(exp);
                            _instrlist.Add(id, expList);
                        }
                    }
                }
            }

            return _instrlist;
        }
    }
}
