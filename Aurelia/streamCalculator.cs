using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurelia
{
    class streamCalculator
    {
        public static void StreamCalculator()
        {
            List<Card> allcards = Database.GetAllCardsInDatabase();
        }
    }
}
