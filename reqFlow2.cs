using MktSrvcAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePriceServer
{
    class reqFlow2
    {
        public static object locker = new object();
        public static ConcurrentQueue<output> _queue = new ConcurrentQueue<output>();
        //public static void requestFlow(output x)
        //{
        //    try
        //    {
        //        _queue.Enqueue(x);
        //        DepthOfBkHndlr depofbkhndlr0 = new MktSrvcAPI.DepthOfBkHndlr((_instr, _ts, _partid, _mod, _numbid, _numask, _bidexch, _askexch, _bidbk, _askbk) => respFlow2.depofbkhndlr(_instr, _ts, _partid, _mod, _numbid, _numask, _bidexch, _askexch, _bidbk, _askbk));
        //        Conn._DepthofBkClient.depofbkhndlr = depofbkhndlr0;
        //        Conn._DepthofBkClient.Subscribe(x.instr);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}

        public static void requestFlow(output x1, output x2)
        {
            try
            {
                _queue.Enqueue(x1);
                _queue.Enqueue(x2);
                DepthOfBkHndlr depofbkhndlr0 = new MktSrvcAPI.DepthOfBkHndlr((_instr, _ts, _partid, _mod, _numbid, _numask, _bidexch, _askexch, _bidbk, _askbk) => respFlow2.depofbkhndlr(_instr, _ts, _partid, _mod, _numbid, _numask, _bidexch, _askexch, _bidbk, _askbk));
                Conn._DepthofBkClient.depofbkhndlr = depofbkhndlr0;
                Conn._DepthofBkClient.Subscribe(x1.instr);
                Conn._DepthofBkClient.Subscribe(x2.instr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
