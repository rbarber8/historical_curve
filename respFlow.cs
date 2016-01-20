using MktSrvcAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePriceServer
{
    class respFlow
    {
        public static object locker2 = new object();
        public static ConcurrentQueue<equity> queue = new ConcurrentQueue<equity>();//equity quote
        public static void dobkUnderhndlr(InstrInfo instr, uint ts, byte partid, int mod, byte numbid, byte numask, byte[] bidexch, byte[] askexch, Quote[] bidbk, Quote[] askbk)
        {
            lock (respFlow.locker2)
            {
                try
                {
                    output x;
                    reqFlow._queue.TryDequeue(out x);
                    equity q = new equity
                    {
                        Instr = instr,
                        Ts = ts,
                        Bidexch = bidexch,
                        Askexch = askexch,
                        Bidbk = bidbk,
                        Askbk = askbk,
                        x = x
                    };
                    queue.Enqueue(q);
                    Console.WriteLine("response: " + x.under + x.callput + "  " + instr.sym);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
