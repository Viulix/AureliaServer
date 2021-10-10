using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurelia
{
    class Levelsystem
    {
        public static class Dicton
        {
            public static Dictionary<ulong, DateTime> Cooldowns = new();
        }
        public static bool CheckUserLevelUp(ulong userid)
        {
            int userLevel = Database.UserLevel(userid, 0);
            int userxp = Database.UserLevel(userid, 1);
            if (Math.Ceiling(Convert.ToDecimal(Math.Pow(Convert.ToDouble(userxp), 0.2))) > userLevel)
            {
                Database.AddUserXpOrLevel(userid);
                return true;
            }
            else return false;
        }
    }
}
