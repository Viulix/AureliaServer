using Discord.Commands;
using Discord;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;
using System.Text;

namespace Aurelia
{
    public class UserCommands : ModuleBase
    {

        public static class MemBase
        {
            public static Dictionary<ulong, (int x, int y, int z)> InvValues = new();
            public static Dictionary<ulong, (string cardid, int price)> SellCardValues = new();
            public static Dictionary<ulong, List<Card>> SellAllCardList = new();
        }
        public struct Idol
        {
            public int internalRarity;
            public bool exists;
            public ulong owner;
            public string idolName;
            public string group;
            public int dance;
            public int voice;
            public int popularity;

        }
        public static Embed UserProfile(SocketUser user)
        {
            Embed builder = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithDescription("***Your Aurelia Stats*** \n Everything about you.")
                .WithTimestamp(DateTime.Now)
                .WithTitle(user.Username)
                .WithColor(Discord.Color.DarkMagenta)
                .WithFooter("", iconUrl: user.GetAvatarUrl())
                .AddField("Cards in inventory:", $"{Database.UserInventoryLength(user.Id)}")
                .AddField("Level | Experience", $"> {Database.UserLevel(user.Id, 0)} | {Database.UserLevel(user.Id, 1)}")
                .Build();
            return builder;

        }
        [Command("reloadCommands")]
        public async Task Reload()
        {
            try
            {
                if (Context.User.Id == 300570127869411329)
                {
                    await CommandHandler.Drop();
                    await Context.Channel.SendMessageAsync("Commands have been reloaded. Please wait a few minutes until the commands are available again.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static Embed Inventory(Discord.WebSocket.SocketUser user)
        {
            if (MemBase.InvValues.Where(x => x.Key == user.Id).Count() == 0)
            {
                MemBase.InvValues.Add(user.Id, (0, 14, 1));
            }
            int x = UserCommands.MemBase.InvValues[user.Id].x;
            int y = UserCommands.MemBase.InvValues[user.Id].y;
            int z = UserCommands.MemBase.InvValues[user.Id].z;
            try
            {
                if (x >= Database.UserInventoryLength(user.Id))
                {
                    y = Database.UserInventoryLength(user.Id);
                    x = y - 15;
                    z = y / 15;
                    Console.WriteLine(x + "|" + y + "|" + z);
                }
                else if (x <= 0 || y <= 0)
                { 
                    x = 0;
                    y = 14;
                    z = 1;
                }
                byte kek = 0;
                MemBase.InvValues[user.Id] = (x, y, z);
                List<string> inv = Database.UserInventory(user.Id, x, y);
                List<string> invIdol = new List<string>();

                decimal d = 15;
                decimal inventoryPages = Math.Ceiling(Convert.ToDecimal(Database.UserInventoryLength(user.Id)) / d);
                string space = "\n";
                var Emb = new EmbedBuilder()
                    .WithTitle("📚 Your Inventory")
                    .WithThumbnailUrl(user.GetAvatarUrl())
                    .WithDescription(String.Join(space, Database.UserInventory(user.Id, kek, x, y)))
                    //.AddField("Cards", String.Join(space, Database.UserInventory(user.Id, kek, x, y)), true)
                    .WithAuthor("👑 " + user.Username)
                    .WithColor(Discord.Color.DarkBlue)
                    .WithFooter($"Page 📑 » {z} | {inventoryPages} «", iconUrl: user.GetAvatarUrl())
                    .WithTimestamp(DateTime.Now)
                    .Build();

                return Emb;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }
        public static ComponentBuilder Inventory(Discord.WebSocket.SocketUser user, bool check)
        {
            bool button1 = false;
            bool button2 = false;
            int x = UserCommands.MemBase.InvValues[user.Id].x;
            int y = UserCommands.MemBase.InvValues[user.Id].y;
            int z = UserCommands.MemBase.InvValues[user.Id].z;
            try
            {
                Console.WriteLine(x + "|" + y + "|" + z);
                if (x <= 0 || y <= 0)
                {
                    button1 = true;
                    x = 0;
                    y = 14;
                    z = 1;
                    Console.WriteLine(1);
                }
                if (y >= Database.UserInventoryLength(user.Id))
                {
                    button2 = true;
                }
                else if (Database.UserInventoryLength(user.Id) <= y)
                {
                    button2 = true;
                    Console.WriteLine(Database.UserInventoryLength(user.Id));
                    x = 0;
                    y = 14;
                    z = 1;
                }
                var builder = new ComponentBuilder().WithButton("⏪", customId: "inventory_back", ButtonStyle.Primary, disabled: button1, row: 0).WithButton("⏩", customId: "inventory_forward", ButtonStyle.Primary, disabled: button2);
                return builder;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }
        public static void SellCard(string cardid, ulong userid, int internalRarity)
        {
            Database.RemoveCard(cardid, userid);
            Database.AddMoneyToUser(userid, idgenerator.GetRarityPrice(internalRarity));
        }
        public static Embed DailyDrop(ulong userid)
        {
            long userTimestamp = Database.UserDailyStamp(userid);
            if (userTimestamp + 86400 <= new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
            {
                // Defining random values that the user receives
                Random rnd = new();
                int xp = rnd.Next(75, 200);
                int coins = rnd.Next(200, 400);
                int diamonds = rnd.Next(1, 3);

                // Adding all values to the user's profile
                Database.AddUserXpOrLevel(userid, xp);
                Database.AddMoneyToUser(userid, coins);
                Database.AddUserDiamonds(userid, diamonds);
                Database.UserUpdateDailyStamp(userid);

                // Make Embed
                Embed emb = new EmbedBuilder()
                    .WithTitle("Your daily drop is here!")
                    .WithColor(Discord.Color.Teal)
                    .WithDescription($"You received: \n \n > **`{xp}`XP** \n > **`{coins}`🪙** \n > **`{diamonds}`💎**")
                    .Build();
                return emb;
            }
            else
            {
                // Determining the time the user has to wait for the next drop
                long waitTime = (userTimestamp + 86400) - new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                int timeHours = Convert.ToInt32(waitTime / 3600);
                int timeMinuts = Convert.ToInt32((waitTime / 60) % 60);

                Embed emb = new EmbedBuilder()
                    .WithTitle("Oh.. Please wait a bit longer...")
                    .WithColor(Discord.Color.Teal)
                    .WithDescription($"Calm down. Please wait `{timeHours}h {timeMinuts}min` until you get your daily drop again!")
                    .Build();
                return emb;
            }
        }
        public static Embed UserAlbums(SocketUser user, string group)
        {
            
            List<object> finalresult = Database.GetUserGroupAlbum(user.Id, group);
            string complete = "**Not completed.**";
            List<string> list1 = new ();   
            List<string> list2 = new ();
            int collected = 0;
            int alltogether = 0;
            foreach (var obj in finalresult)
            {
                foreach (var prop in obj.GetType().GetProperties())
                {
                    string yesOrNo = "No";
                    switch (prop.GetValue(obj))
                    {
                        case 1:
                            yesOrNo = "Yes";
                            collected += 1;
                            break;
                        default:
                            break;
                    }
                    if (prop.Name == "Complete")
                    {
                        switch (prop.GetValue(obj))
                        {
                            case 1:
                                complete = "**Completed.**";
                                collected -= 1;
                                break;
                            default:
                                break;
                        }
                    }
                    else 
                    {                   
                        list1.Add($"> *{prop.Name}*");
                        list2.Add($"> **{yesOrNo}**");
                        alltogether += 1; 
                    }
                }
            }
            var emb = new EmbedBuilder()
                .WithTitle(group)
                .WithAuthor("👑 " + user.Username)
                .WithDescription($"Did you complete the whole album? \n > {complete}")
                .AddField("Idol", String.Join("\n", list1), true)
                .AddField("Collected?", String.Join("\n", list2), true)
                .WithColor(Discord.Color.Teal)
                .WithFooter($"» {collected} | {alltogether} «", iconUrl: user.GetAvatarUrl())
                .WithTimestamp(DateTime.Now)
                .Build();
            
            return emb; 
        }
    }
}

