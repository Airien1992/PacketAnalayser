using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketAnalayser
{
    class Packet
    {
        private string protocol;
        private string source;

        public string Protocol
        {
            get { return protocol; }
            set
            {
                protocol = value;
            }
        }
        
    }
}
