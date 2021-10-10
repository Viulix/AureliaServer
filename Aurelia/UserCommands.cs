using Discord.Commands;
using Discord;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

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
                byte kek = 0;
                List<string> inv = Database.UserInventory(user.Id, x, y);
                List<string> invIdol = new List<string>();

                decimal d = 14;
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

                if (x <= 0 || y <= 0)
                { 
                    x = 0;
                    y = 14;
                    z = 1;
                }
                MemBase.InvValues[user.Id] = (x, y, z);
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
                    Console.WriteLine(2);
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
            Console.WriteLine(idgenerator.GetRarityPrice(internalRarity));
            Database.AddMoneyToUser(userid, idgenerator.GetRarityPrice(internalRarity));
            Console.WriteLine("Done.");
        }
    }
}
