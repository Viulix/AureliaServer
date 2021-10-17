using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurelia
{
    class Company
    {
        public string id { get; set; }
        public ulong owner { get; set; }
        public string name { get; set; }
        public List<string> groups { get; set; }
        public int producerClass { get; set; }
        public long value { get; set; }
        public int rating { get; set; }
        public long streams { get; set; }
    }
}
