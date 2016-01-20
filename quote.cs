using MktSrvcAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePriceServer
{
    class quote
    {
        internal InstrInfo Instr { get; set; }
        internal uint Ts { get; set; }
        internal byte[] Bidexch { get; set; }
        internal byte[] Askexch { get; set; }
        internal Quote[] Bidbk { get; set; }
        internal Quote[] Askbk { get; set; }
        internal output x { get; set; }
    }
}
