using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Aurelia
{
    public class User
    {
        [BsonId]
        public ulong id { get; set; }
        public long dailystamp { get; set; }
        public long joined { get; set; }
        public int lvl { get; set; }
        public int xp { get; set; }
        public int luck { get; set; }
        public int balance { get; set; }
        public int diamonds { get; set; }
        public List<string> inventory { get; set; }
        public Album albums { get; set; }
    }

    public class Album
    {
        public BLACKPINK BLACKPINK { get; set; }
        public TWICE TWICE { get; set; }
    }

    public class BLACKPINK
    {
        public int Complete { get; set; }
        public int Lisa { get; set; }
        public int Jennie { get; set; }
        public int Jisoo { get; set; }
        public int Rose { get; set; }
    }
    
    public class TWICE  
    {
        public int Complete { get; set; }
        public int Sana { get; set; }
        public int Nayeon { get; set; }
        public int Momo { get; set; }
        public int Mina { get; set; }
        public int Chaeyoung { get; set; }
        public int Dahyun { get; set; }
        public int Jihyo { get; set; }
        public int Tzuyu { get; set; }
        public int Jeongyeon { get; set; }
    }
}