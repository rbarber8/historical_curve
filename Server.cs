using MktSrvcAPI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LivePriceServer
{
    class Server
    {
        public static void start()
        {
            Thread a = new Thread(() => updateE(respFlow.queue));
            Thread b = new Thread(() => updateQ(respFlow2.queue));
            a.Start();
            b.Start();
        }

        public static void updateE(ConcurrentQueue<equity> queue)
        {
            ThreadPool.SetMinThreads(10, 10);
            while (true)
            {
                if (queue.Count != 0)
                {
                    equity q;
                    queue.TryDequeue(out q);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(equityHndlr.NewMethod), q);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

        public static void updateQ(ConcurrentQueue<quote> queue)
        {
            ThreadPool.SetMinThreads(400, 400);
            while (true)
            {
                if (queue.Count > 0)
                {
                    quote q;
                    queue.TryDequeue(out q);
                    ThreadPool.QueueUserWorkItem(new WaitCallback(quoteHndlr.NewMethod), q);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }
    }
}
