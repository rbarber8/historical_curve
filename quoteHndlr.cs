using MktSrvcAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LivePriceServer
{
    class quoteHndlr
    {
        public static void NewMethod(object q)
        {
            quote info = (quote)q;
            if (info != null)
            {
                subtask(info.Instr, info.Ts, info.Bidexch, info.Askexch, info.Bidbk, info.Askbk, info.x);
            }
        }

        public static void subtask(InstrInfo instr, uint ts, byte[] bidexch, byte[] askexch, Quote[] bidbk, Quote[] askbk, output x)
        {
            string _quoteinstrument = instr.sym + instr.maturity + instr.callput + instr.strike;


            #region Quote Filtering
            List<FilteredQuote> _Bidbk = new List<FilteredQuote>();
            List<FilteredQuote> _Askbk = new List<FilteredQuote>();
            List<char> _Bidexch = new List<char>();
            List<char> _Askexch = new List<char>();
            calib.QuoteFiltering(bidexch, bidbk, _Bidbk, _Bidexch);
            calib.QuoteFiltering(askexch, askbk, _Askbk, _Askexch);
            #endregion

            x._Wbidsz = calib.CalculateWsz(_Bidbk);
            x._Wasksz = calib.CalculateWsz(_Askbk);

            if (x._Wbidsz == 0)
            {
                x._Wbidprc = 0;
            }
            else
            {
                x._Wbidprc = calib.CalculateWprc(_Bidbk) / x._Wbidsz;
            }

            if (x._Wasksz == 0)
            {
                x._Waskprc = 0;
            }
            else
            {
                x._Waskprc = calib.CalculateWprc(_Askbk) / x._Wasksz;
            }


            if (x._Wbidprc == 0)
            {
                //low the weight by (wbidprc = 0 + waskprc + askprc[0]) / 3
                x._Wmidpt = (x._Waskprc + _Askbk[0]._prc) / 3;
                x._Vwap = x._Wmidpt;
            }
            else if (x._Waskprc == 0)
            {
                x._Wmidpt = (x._Wbidprc + _Bidbk[0]._prc) / 3;
                x._Vwap = x._Wmidpt;
            }
            else
            {
                x._Wmidpt = (x._Wbidprc + x._Waskprc) / 2;
                x._Vwap = calib.CalculateVWAP(_Bidbk, _Askbk);
            }
            x.svwap = (x._Wmidpt + x._Vwap) / 2;

            Console.WriteLine(_quoteinstrument + "  " + x.svwap);
            //asynsocketclient.StartSend("10.0.7.218",15000, x.stock + "|" + x.svwap);
            //sclient.start("10.0.7.182", x.stock + "|" + x.svwap);



            if (x.flag == true)
            {
                sclient.start(x.ip, x.stock + "|" + x.svwap);
            }
            else
            {
                x.flag = true;
                if (x.index == 1)
                {
                    while (x.nearby.flag != true)
                    {
                        Thread.Sleep(30);
                    }

                    if (x.instr.strike > x.stock)
                    {
                        float svwap = x.svwap * (x.stock - x.nearby.instr.strike) / (x.instr.strike - x.nearby.instr.strike) + x.nearby.svwap * (x.instr.strike - x.stock) / (x.instr.strike - x.nearby.instr.strike);
                        sclient.start(x.ip, x.stock + "|" + svwap);
                    }
                }
            }

            Console.WriteLine("ip: " + x.ip);
        }
    }
}
