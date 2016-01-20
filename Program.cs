using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LivePriceServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                TimeSpan t = new TimeSpan(8, 20, 0);
                while (true)
                {
                    if (DateTime.Now.TimeOfDay >= t)
                    {
                        Conn.connDB();
                        Server.start();
                        sserver.start("10.0.7.202");
                        break;
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }

                TimeSpan t2 = new TimeSpan(20, 00, 0);
                while (true)
                {
                    if (DateTime.Now.TimeOfDay >= t2)
                    {
                        System.Environment.Exit(1);
                    }
                    else
                    {
                        Thread.Sleep(600000);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
