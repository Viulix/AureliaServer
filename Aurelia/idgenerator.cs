using Discord;
using System;
using System.Linq;
using System.Text;

namespace Aurelia
{
    public class idgenerator
    {
        public static dynamic CardIdGenerator(bool check, int identifier)
        {
            string cardid = "";
            while (check is true)
            {
                StringBuilder builder = new StringBuilder();
                Enumerable
                   .Range(65, 26)
                    .Select(e => ((char)e).ToString())
                    .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                    .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                    .OrderBy(e => Guid.NewGuid())
                    .Take(8)
                    .ToList().ForEach(e => builder.Append(e));
                cardid = builder.ToString().ToUpper();
                bool cardcheck = false;
                if (identifier == 0)
                {
                    cardcheck = Database.CheckCardId(cardid);
                }
                if (identifier == 1)
                {
                    cardcheck = companies.IsSongNameAvailable(cardid);
                }

                if (cardcheck is false)
                {
                    check = false;
                }
            }
            return cardid;
        }
        public static dynamic raritySetter()
        {
            Random rnd = new Random();
            int rarityNumber = rnd.Next(1, 1000);
            int rarity;

            switch (rarityNumber)
            {
                case >= 990 when rarityNumber < 1000:
                    rarity = 5;
                    break;
                case >= 950 when rarityNumber < 990:
                    rarity = 4;
                    break;
                case >= 880 when rarityNumber < 950:
                    rarity = 3;
                    break;
                case >= 650 when rarityNumber < 880:
                    rarity = 2;
                    break;
                default:
                    rarity = 1;
                    break;
            }
            return rarity;
        }
        public static dynamic rarityTranslator(int rarityNum, int col)
        {
            string rarity = "";
            Color colour = Color.DarkGrey;
            switch(rarityNum)
            {
                case 1:
                    rarity = "Common";
                    colour = Color.DarkGrey;
                    break;
                case 2:
                    rarity = "Uncommon";
                    colour = new Color(0x00ff00);
                    break;
                case 3:
                    rarity = "Rare";
                    colour = new Color(0x0057ff);
                    break; 
                case 4:
                    rarity = "Epic";
                    colour = new Color(0xFb00ff);
                    break;
                case 5:
                    rarity = "Legendary";
                    colour = new Color(0xFfa300);
                    break;
            }
            if (col == 1)
            {
                return colour;
            }
            return rarity;
        }
        public static dynamic rarityTranslator(int rarityNum, byte em)
        {
            string rarity = "Something went wrong...";
            switch (rarityNum)
            {
                case 1:
                    rarity = "🖤";
                    break;
                case 2:
                    rarity = "💚";
                    break;
                case 3:
                    rarity = "💙";
                    break;
                case 4:
                    rarity = "💜";
                    break;
                case 5:
                    rarity = "🧡";
                    break;
            }
            return rarity;
        }
        public static dynamic rarityTranslator(int rarityNum, bool em)
        {
            string rarity = "";
            switch (rarityNum)
            {
                case 1:
                    rarity = "🔥";
                    break;
                case 2:
                    rarity = "🔥🔥";
                    break;
                case 3:
                    rarity = "🔥🔥🔥";
                    break;
                case 4:
                    rarity = "🔥🔥🔥🔥";
                    break;
                case 5:
                    rarity = "🔥🔥🔥🔥🔥";
                    break;
            }
            return rarity;
        }
        public static dynamic GetRarityPrice(int rarityNum)
        {
            int rarity = 0;
            switch (rarityNum)
            {
                case 1:
                    rarity = 10;
                    break;
                case 2:
                    rarity = 25;
                    break;
                case 3:
                    rarity = 50;
                    break;
                case 4:
                    rarity = 175;
                    break;
                case 5:
                    rarity = 500;
                    break;
            }
            return rarity;
        }
        public static dynamic NumberInLetterTranslator(int rarityNum)
        {
            string rarity = "E";
            switch (rarityNum)
            {
                case 1:
                    rarity = "E";
                    break;
                case 2:
                    rarity = "D";
                    break;
                case 3:
                    rarity = "C";
                    break;
                case 4:
                    rarity = "B";
                    break;
                case 5:
                    rarity = "A";
                    break;
                case 6:
                    rarity = "A+";
                    break;
                case 7:
                    rarity = "S";
                    break;
                case 8:
                    rarity = "S+";
                    break;
                case 9:
                    rarity = "S++";
                    break;
            }
            return rarity;
        }
    }
}
