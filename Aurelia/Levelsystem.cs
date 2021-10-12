using System;
using System.Collections.Generic;


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
            decimal newLevel = Math.Floor(Convert.ToDecimal(Math.Pow(Convert.ToDouble(userxp), 0.2)));
            if (newLevel > userLevel)
            {
                Database.AddUserLevel(userid, Convert.ToInt32(newLevel));
                return true;
            }
            else return false;
        }
    }
}
