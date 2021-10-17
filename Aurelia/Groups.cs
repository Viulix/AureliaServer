using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurelia
{
    class Group
    {
        public string id { get; set; }
        public int sizelimit { get; set; }
        public DateTime DateOfCreation { get; set; }
        public long SongCooldown { get; set; }
        public int streams { get; set; }
        public int popularity { get; set; }
        public List<string> songs { get; set; }
        public ulong owner { get; set; }
        public string name { get; set; }
        public string company { get; set; }
        public List<string> idols { get; set; }
    }
}
