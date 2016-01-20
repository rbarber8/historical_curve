using MktSrvcAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivePriceServer
{
    public class output
    {
        public string under { get; set; }
        public int exp { get; set; }
        public string callput { get; set; }
        public float stock { get; set; }
        public float svwap { get; set; }
        public InstrInfo instr { get; set; }

        public uint _BidSz { get; set; }
        public uint _AskSz { get; set; }
        public float _Wbidprc { get; set; }
        public float _Waskprc { get; set; }
        public uint _Wbidsz { get; set; }
        public uint _Wasksz { get; set; }
        public float _Vwap { get; set; }
        public float _Wmidpt { get; set; }
        public bool flag { get; set; }
        public int index { get; set; }
        public output nearby { get; set; }
        public string ip { get; set; }
    }
}
