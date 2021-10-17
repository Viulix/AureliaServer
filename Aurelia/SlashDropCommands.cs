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

                // Create company
                var createCommand = new SlashCommandBuilder()
                                   .WithName("create")
                                   .WithDescription("Create companies, groups or songs!")
                                   .AddOption(new SlashCommandOptionBuilder()
                                       .WithName("company")
                                       .WithDescription("Creates a new company")
                                       .WithType(ApplicationCommandOptionType.SubCommand)
                                       .AddOption("name", ApplicationCommandOptionType.String, "The name of your company", required: true))
                                   .AddOption(new SlashCommandOptionBuilder()
                                       .WithName("group")
                                       .WithDescription("Creates a new group")
                                       .WithType(ApplicationCommandOptionType.SubCommand)
                                       .AddOption("name", ApplicationCommandOptionType.String, "The name of your group", required: true))
                                   .AddOption(new SlashCommandOptionBuilder()
                                       .WithName("song")
                                       .WithDescription("Creates a new song")
                                       .AddOption("groupname", ApplicationCommandOptionType.String, "The name of the group that should make a new song!", required: true)
                                       .AddOption("songname", ApplicationCommandOptionType.String, "What's the name of the new song?", required: true)
                                       .WithType(ApplicationCommandOptionType.SubCommand));
                list.Add(createCommand);
                // Add Idol To Group
                var AddIdolCommand = new SlashCommandBuilder();
                AddIdolCommand.WithName("addidol");
                AddIdolCommand.WithDescription("Adds an card to your group!");
                AddIdolCommand.AddOption("groupname", ApplicationCommandOptionType.String, "The name of the group you want to add the card to!", required: true);
                AddIdolCommand.AddOption("cardid", ApplicationCommandOptionType.String, "The cardid for the card you want to add!", required: true);
                list.Add(AddIdolCommand);

                // Show Group
                var showGroupCommand = new SlashCommandBuilder()
                    .WithName("showgroup")
                    .WithDescription("Shows basic information about a group")
                    .AddOption("name", ApplicationCommandOptionType.String, "The name of the group you want to view", required: true);
                list.Add(showGroupCommand);

                // Balance
                var balanceCommand = new SlashCommandBuilder();
                balanceCommand.WithName("balance");
                balanceCommand.WithDescription("Shows your Aurelia bank account!");
                list.Add(balanceCommand);

                // View Company
                var mycompanyCommand = new SlashCommandBuilder();
                mycompanyCommand.WithName("mycompany");
                mycompanyCommand.WithDescription("Shows the basic stats of your company!");
                list.Add(mycompanyCommand);

                // Daily
                var dailyDropCommand = new SlashCommandBuilder();
                dailyDropCommand.WithName("daily");
                dailyDropCommand.WithDescription("Lets you drop a daily gift!");
                list.Add(dailyDropCommand);

                // Album Command
                var albumCommand = new SlashCommandBuilder();
                albumCommand.WithName("album");
                albumCommand.WithDescription("Shows you one of your albums!");
                albumCommand.AddOption(new SlashCommandOptionBuilder()
                        .WithName("group")
                        .WithDescription("Which album do you want to view?")
                        .WithRequired(true)
                        .AddChoice("TWICE", "TWICE")
                        .AddChoice("BLACKPINK", "BLACKPINK")
                        .WithType(ApplicationCommandOptionType.String));
                list.Add(albumCommand);

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
