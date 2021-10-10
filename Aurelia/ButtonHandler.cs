using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurelia
{
    class ButtonHandler
    {
        public static Embed InventoryForward(SocketGuildUser user)
        {


            // Get the inventory ranges and pages from the dictionary

            int x = UserCommands.MemBase.InvValues[user.Id].x;
            int y = UserCommands.MemBase.InvValues[user.Id].y;
            int z = UserCommands.MemBase.InvValues[user.Id].z;

            x = x + 14;
            y = y + 14;
            z = z + 1;
            decimal d = 14;
            decimal inventoryPages = Math.Ceiling(Convert.ToDecimal(Database.UserInventoryLength(user.Id)) / d);


            if (x <= 0 || y <= 0)
            {
                x = 0;
                y = 14;
                z = 1;
            }

            List<string> inv = Database.UserInventory(user.Id, x, y);
            List<string> invIdol = new List<string>();

            byte kek = 0;
            string space = "\n";
            UserCommands.MemBase.InvValues[user.Id] = (x, y, z);
            var Emb = new EmbedBuilder()
                .WithTitle("Inventory")
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithDescription("Here you can organise your cards.")
                .AddField("Cards", String.Join(space, Database.UserInventory(user.Id, kek, x, y)), true)
                .WithAuthor(user.Username)
                .WithColor(Discord.Color.DarkBlue)
                .WithFooter($"Page » {z} | {inventoryPages} «", iconUrl: user.GetAvatarUrl())
                .WithTimestamp(DateTime.Now)
                .Build();
            return Emb;
        }
        public static ComponentBuilder InventoryForward(SocketGuildUser user, int x, int y, int z)
        {

            var builder = new ComponentBuilder().WithButton("⏪", customId: "inventory_back", ButtonStyle.Primary, disabled: false, row: 0)
                .WithButton("⏩", customId: "inventory_forward", ButtonStyle.Primary, disabled: false, row: 0);
            x = x + 14;
            y = y + 14;
            z = z + 1;

            if (x <= 0 || y <= 0)
            {
                builder = new ComponentBuilder().WithButton("⏪", customId: "inventory_back", ButtonStyle.Primary, disabled: true, row: 0)
                    .WithButton("⏩", customId: "inventory_forward", ButtonStyle.Primary, disabled: false, row: 0); ;
                x = 0;
                y = 14;
                z = 1;
            }
            if (x <= 0 || y <= 0 && y >= Database.UserInventoryLength(user.Id))
            {
                builder = new ComponentBuilder().WithButton("⏪", customId: "inventory_back", ButtonStyle.Primary, disabled: true, row: 0)
                    .WithButton("⏩", customId: "inventory_forward", ButtonStyle.Primary, disabled: true, row: 0); ;
                x = 0;
                y = 14;
                z = 1;
            }
            if (y >= Database.UserInventoryLength(user.Id))
            {
                builder = new ComponentBuilder().WithButton("⏪", customId: "inventory_back", ButtonStyle.Primary, disabled: false, row: 0)
                    .WithButton("⏩", customId: "inventory_forward", ButtonStyle.Primary, disabled: true, row: 0);
            }
            UserCommands.MemBase.InvValues[user.Id] = (x, y, z);
            return builder;
        }
        public static Embed InventoryBackwards(SocketGuildUser user)
        {
            // Get the inventory ranges and pages from the dictionary
            
            int x = UserCommands.MemBase.InvValues[user.Id].x;
            int y = UserCommands.MemBase.InvValues[user.Id].y;
            int z = UserCommands.MemBase.InvValues[user.Id].z;

            decimal d = 14;
            decimal inventoryPages = Math.Ceiling(Convert.ToDecimal(Database.UserInventoryLength(user.Id)) / d);
            x = x - 14;
            y = y - 14;
            z = z - 1;
            List<string> inv = Database.UserInventory(user.Id, x, y);
            List<string> invIdol = new List<string>();

            byte kek = 0;
            string space = "\n";

            if (x <= 0 || y <= 0)
            {
                x = 0;
                y = 14;
                z = 1;
            }

            UserCommands.MemBase.InvValues[user.Id] = (x, y, z);
            var Emb = new EmbedBuilder()
                .WithTitle("Inventory")
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithDescription("Here you can organise your cards.")
                .AddField("Cards", String.Join(space, Database.UserInventory(user.Id, kek, x, y)), true)
                .WithAuthor(user.Username)
                .WithColor(Discord.Color.DarkBlue)
                .WithFooter($"Page » {z} | {inventoryPages} «", iconUrl: user.GetAvatarUrl())
                .WithTimestamp(DateTime.Now)
                .Build();
            return Emb;
        }
        public static ComponentBuilder InventoryBackwards(SocketGuildUser user, int x, int y, int z)
        {
            var builder = new ComponentBuilder().WithButton("⏪", customId: "inventory_back", ButtonStyle.Primary, disabled: false, row: 0)
                .WithButton("⏩", customId: "inventory_forward", ButtonStyle.Primary, disabled: false, row: 0);
            decimal d = 14;
            decimal inventoryPages = Math.Ceiling(Convert.ToDecimal(Database.UserInventoryLength(user.Id)) / d);
            
            x = x - 14;
            y = y - 14;
            z = z - 1;
            
            List<string> inv = Database.UserInventory(user.Id, x, y);
            List<string> invIdol = new List<string>();


            if (y >= Database.UserInventoryLength(user.Id))
            {
                builder = new ComponentBuilder().WithButton("⏪", customId: "inventory_back", ButtonStyle.Primary, row: 0)
                    .WithButton("⏩", customId: "inventory_forward", ButtonStyle.Primary, disabled: false, row: 0);
            }
            else if (x <= 0 || y <= 0)
            {
                builder = new ComponentBuilder().WithButton("⏪", customId: "inventory_back", ButtonStyle.Primary, disabled: true, row: 0)
                    .WithButton("⏩", customId: "inventory_forward", ButtonStyle.Primary, disabled: false, row: 0);
                x = 0;
                y = 14;
                z = 1;
            }
            else
            {
                builder = new ComponentBuilder().WithButton("⏪", customId: "inventory_back", ButtonStyle.Primary, disabled: false, row: 0)
                    .WithButton("⏩", customId: "inventory_forward", ButtonStyle.Primary, disabled: false, row: 0);
            }
            UserCommands.MemBase.InvValues[user.Id] = (x, y, z);
            return builder;
         
        }
        public static ComponentBuilder AcceptAndDenyButtonsSell(int id)
        {
            var accept_deny_Button = new ComponentBuilder()
                .WithButton("Accept", customId: $"{id}accept_button_sell", ButtonStyle.Success, disabled: false, row: 0)
                .WithButton("Deny", customId: "deny_button", ButtonStyle.Danger, disabled: false, row: 0);
            return accept_deny_Button;
        }
    }
}
