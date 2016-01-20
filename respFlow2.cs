using MktSrvcAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePriceServer
{
    class respFlow2
    {
        public static object locker0 = new object();
        public static ConcurrentQueue<quote> queue = new ConcurrentQueue<quote>();//surrounding quote
        public static void depofbkhndlr(InstrInfo instr, uint ts, byte partid, int mod, byte numbid, byte numask, byte[] bidexch, byte[] askexch, Quote[] bidbk, Quote[] askbk)
        {
            lock (respFlow2.locker0)
            {
                //System.Diagnostics.Debug.WriteLine("111 " + ((ConcurrentQueue<output>)_Dict[0][1]).Count);
                try
                {
                    output x;
                    reqFlow2._queue.TryDequeue(out x);
                    quote q = new quote
                    {
                        Instr = instr,
                        Ts = ts,
                        Bidexch = bidexch,
                        Askexch = askexch,
                        Bidbk = bidbk,
                        Askbk = askbk,
                        x = x,
                    };
                    queue.Enqueue(q);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
