using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurelia
{
    class Song
    {
        public string Id { get; set; }
        public ulong Owner { get; set; }
        public string Group { get; set; }
        public bool Hit { get; set; }
        public string Name { get; set; }
        public bool Released { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Songquality { get; set; }
        public int Choreoquality { get; set; }
        public long DailyStreams { get; set; }
        public long TotalStreams { get; set; }
    }
}
