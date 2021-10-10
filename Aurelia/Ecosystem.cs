using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurelia
{
    class Ecosystem
    {
        public static void SellCardAccept(ulong userid)
        {
            int price = 0;
            string cardid = "";
            if (UserCommands.MemBase.SellCardValues.Where(x => x.Key == userid).Count() == 1)
            {
                cardid = UserCommands.MemBase.SellCardValues[userid].cardid;
                price = UserCommands.MemBase.SellCardValues[userid].price;
            }
            else
            {
                
                return;
            }
            try
            {
                Console.WriteLine(price);
                UserCommands.SellCard(cardid, userid, price);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
