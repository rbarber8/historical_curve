using MktSrvcAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePriceServer
{
    class reqFlow
    {
        public static object locker0 = new object();
        public static ConcurrentQueue<output> _queue = new ConcurrentQueue<output>();// dict store all surrounding Info
        public static void requestFlow(output x)
        {
            lock (reqFlow.locker0)
            {
                try
                {
                    _queue.Enqueue(x);
                    DepthOfBkHndlr dobkUnderhndlr = new MktSrvcAPI.DepthOfBkHndlr((_instr, _ts, _partid, _mod, _numbid, _numask, _bidexch, _askexch, _bidbk, _askbk) => respFlow.dobkUnderhndlr(_instr, _ts, _partid, _mod, _numbid, _numask, _bidexch, _askexch, _bidbk, _askbk));
                    Conn._dobkUnderClient.depofbkhndlr = dobkUnderhndlr;

                    Conn._dobkUnderClient.Subscribe(new InstrInfo
                    {
                        sym = x.under,
                        type = InstrInfo.EType.EQUITY,
                    });
                    //Console.WriteLine("request: " + x.under + x.callput);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                //Console.WriteLine("000 "+((ConcurrentQueue<output>)_Dict[0][6]).Count);
            }
        }
    }
}
