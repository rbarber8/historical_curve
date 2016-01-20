using MktSrvcAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePriceServer
{
    class equityHndlr
    {
        public static object locker = new object();
        public static void NewMethod(object q)
        {
            equity info = (equity)q;
            if (info != null)
            {
                subtask(info.Instr, info.Ts, info.Bidexch, info.Askexch, info.Bidbk, info.Askbk, info.x);
            }
        }

        public static void subtask(InstrInfo instr, uint ts, byte[] bidexch, byte[] askexch, Quote[] bidbk, Quote[] askbk, output x)
        {
            lock (locker)
            {
                x.instr = new InstrInfo
                {
                    sym = x.under,
                    maturity = x.exp,
                    callput = (MktSrvcAPI.InstrInfo.ECallPut)(x.callput == "CALL" ? 1 : 0),
                    type = InstrInfo.EType.OPTION
                };


                if (bidexch != null && askexch != null)
                {
                    x.stock = (bidbk[0].prc + askbk[0].prc) / 2;
                }
                else if (bidexch != null && askexch == null)
                {
                    x.stock = bidbk[0].prc;
                }
                else if (bidexch == null && askexch != null)
                {
                    x.stock = askbk[0].prc;
                }
                else
                {
                    x.stock = 0.0f;
                }

                Console.WriteLine(x.under + "   " + x.stock);


                //ATM
                List<float> strikes = Conn.Dict[x.under + x.exp + x.callput];
                strikes.Sort();
                if (strikes[0] < x.stock && strikes[strikes.Count - 1] > x.stock)
                {
                    foreach (float s in strikes)
                    {
                        if (s - x.stock >= 0)
                        {

                            if (s - x.stock == 0)
                            {
                                x.instr.strike = s;
                                x.flag = true;
                                reqFlow2.requestFlow(x, x);
                            }
                            else
                            {
                                output y = new output();
                                x.instr.strike = s;
                                y.instr = new InstrInfo
                                {
                                    sym = x.under,
                                    maturity = x.exp,
                                    callput = (MktSrvcAPI.InstrInfo.ECallPut)(x.callput == "CALL" ? 1 : 0),
                                    type = InstrInfo.EType.OPTION
                                };
                                y.under = x.under;
                                y.exp = x.exp;
                                y.callput = x.callput;
                                y.ip = x.ip;
                                y.instr.strike = strikes[strikes.IndexOf(s) - 1];
                                x.flag = false;
                                y.flag = false;
                                x.nearby = y;
                                y.nearby = x;
                                x.index = 1;
                                y.index = 0;
                                reqFlow2.requestFlow(y, x);
                            }

                            break;
                        }
                    }
                }
                else if (strikes[0] == x.stock)
                {
                    x.instr.strike = strikes[0];
                    reqFlow2.requestFlow(x, x);
                }
                else if (strikes[strikes.Count - 1] == x.stock)
                {
                    x.instr.strike = strikes[strikes.Count - 1];
                    reqFlow2.requestFlow(x, x);
                }
                else
                {
                    //pass
                }
            }
        }
    }
}
