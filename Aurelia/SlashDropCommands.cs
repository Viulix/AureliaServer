using Discord;
using System;
using System.Collections.Generic;

namespace Aurelia
{
    class SlashDropCommands
    {
        public static List<SlashCommandBuilder> SlashCommands()
        {
            try
            {

            List<SlashCommandBuilder> list = new List<SlashCommandBuilder>();
            // Drop Command
            var dropCommand = new SlashCommandBuilder();
            dropCommand.WithName("drop");
            dropCommand.WithDescription("Drops a card!");
            list.Add(dropCommand);

            // Inventory Command
            var inventoryCommand = new SlashCommandBuilder();
            inventoryCommand.WithName("inventory");
            inventoryCommand.WithDescription("Access your inventory with this command!");
            inventoryCommand.AddOption("private", ApplicationCommandOptionType.Boolean, "Tell me, if I everyone should see your inventory or just you!", required: false);
            list.Add(inventoryCommand);

            // Sell Command
            var sellCommand = new SlashCommandBuilder();
            sellCommand.WithName("sell");
            sellCommand.WithDescription("Sells cards from your inventory for coins!");
            sellCommand.AddOption("cardid", ApplicationCommandOptionType.String, "Sells one card with a specific Id", required: true);
            list.Add(sellCommand);

            // Sellall Command
            var sellAllCommand = new SlashCommandBuilder();
            sellAllCommand.WithName("sellall");
            sellAllCommand.WithDescription("Sells all cards (max 15. per use) with a specific rarity!");
            sellAllCommand.AddOption(new SlashCommandOptionBuilder()
                    .WithName("rarity")
                    .WithDescription("Which rarity do you want to sell?")
                    .WithRequired(true)
                    .AddChoice("common", 1)
                    .AddChoice("uncommon", 2)
                    .AddChoice("rare", 3)
                    .AddChoice("epic", 4)
                    .AddChoice("legendary", 5)
                    .WithType(ApplicationCommandOptionType.Integer));
            list.Add(sellAllCommand);

            // View Command
            var viewCommand = new SlashCommandBuilder();
            viewCommand.WithName("view");
            viewCommand.WithDescription("Lets you view cards of your inventory or of other players!");
            viewCommand.AddOption("cardid", ApplicationCommandOptionType.String,"The card Id I should look for!", required: true);
            list.Add(viewCommand);

            // Profile Command
            var profileCommand = new SlashCommandBuilder();
            profileCommand.WithName("profile");
            profileCommand.WithDescription("Reveals information about yourself (privately)");
            list.Add(profileCommand);
       
            // Money
            var balanceCommand = new SlashCommandBuilder();
            balanceCommand.WithName("balance");
            balanceCommand.WithDescription("Shows your Aurelia bank account!");
            list.Add(balanceCommand);

                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }
    }
}
