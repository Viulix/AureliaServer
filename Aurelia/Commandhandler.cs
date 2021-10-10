using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Aurelia
{
    public class CommandHandler
    {
        public static DiscordSocketClient _client;
        private readonly CommandService _commands;
        public static IServiceProvider _provider;

        // Test
        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider provider)

        {

            _client = client;
            _commands = commands;
            _client.MessageReceived += OnMessageReceived;
            _provider = provider;
            _client.InteractionCreated += Client_InteractionCreated;
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg.Author.IsBot) return;
            var context = new SocketCommandContext(_client, msg);
            int pos = 0;
            if (msg.HasStringPrefix(".", ref pos) || msg.HasMentionPrefix(_client.CurrentUser, ref pos))
            {
                try
                {
                    await _commands.ExecuteAsync(context, pos, _provider);
                    return;
                }
                catch (Exception ex)
                {
                    await context.Channel.SendMessageAsync("> Whoops... Something went wrong! Did you enter a wrong command?");
                    Console.WriteLine(ex);
                    return;
                }
            }
            else
            {
                try
                {
                    bool check = Database.CheckUser(msg.Author.Id);
                    if (check is false)
                    {
                        Database.AddUser(msg.Author.Id);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return;
                }
                Random rnd = new Random();
                bool access = false;
                if (Levelsystem.Dicton.Cooldowns.Where(x => x.Key == msg.Author.Id).Count() == 0)
                {
                    Levelsystem.Dicton.Cooldowns.Add(msg.Author.Id, DateTime.UtcNow);
                    access = true;
                }
                else
                {
                    var UserTimestamp = Levelsystem.Dicton.Cooldowns[msg.Author.Id];
                    if (UserTimestamp.AddSeconds(15) < DateTime.UtcNow)
                    {
                        access = true;
                    }
                    else
                    {
                        access = false;
                    }
                }
                if (access is true)
                {
                    Levelsystem.Dicton.Cooldowns[msg.Author.Id] = DateTime.UtcNow;
                    Database.AddUserXpOrLevel(msg.Author.Id, rnd.Next(20, 70));
                    bool Levelcheck = Levelsystem.CheckUserLevelUp(msg.Author.Id);
                    int UserLevel = Database.UserLevel(msg.Author.Id, 0);

                    if (Levelcheck is true)
                    {
                        await context.Channel.SendMessageAsync($"> {msg.Author.Mention} you just leveled up to **level {UserLevel}**!");
                    }
                }
            }
        }

        private async Task Client_InteractionCreated(SocketInteraction interaction)
        {

            switch (interaction)
            {
                case SocketSlashCommand commandInteraction:
                    await MySlashCommandHandler(commandInteraction);
                    break;
                case SocketMessageComponent componentInteraction:
                    await MyMessageComponentHandler(componentInteraction);
                    break;
                default:
                    break;
            }
        }
        private async Task MySlashCommandHandler(SocketSlashCommand interaction)
        {

            try
            {
                int check = DropCommands.UserBasisCheck(interaction.User);
                switch (check)
                {
                    case 0:
                        break;
                    case 1:
                        Database.AddUser(interaction.User.Id);
                        return;
                }
                if (interaction.Data.Name == "drop")
                {
                    _ = interaction.DeferAsync();
                    if (check == 2)
                    {
                        await interaction.FollowupAsync("> **You have too many cards in your inventory. Please remove some before you drop again!**");
                        return;
                    }
                    string cardid = DropCommands.randomIdoldInfo(interaction.User, 1);
                    int internalRarity = DropCommands.randomIdoldInfo(interaction.User, 2);
                    string rarity = DropCommands.randomIdoldInfo(interaction.User, 3);
                    List<string> idol = DropCommands.randomIdoldInfo(interaction.User, 4);
                    Embed emb = DropCommands.Drop(interaction.User, cardid.Remove(0, 1), internalRarity, idgenerator.rarityTranslator(internalRarity, 0), idol);
                    Console.WriteLine($"assets/droppedCards/{cardid.Remove(0, 1)}1card.png");
                    Database.AlbumAddCard(interaction.User.Id, idol[0], idol[2]);
                    _ = interaction.FollowupWithFileAsync(null, filePath: $"assets/droppedCards/{cardid.Remove(0, 1)}1card.png", fileName: $"{cardid.Remove(0, 1)}1card.png", embed: emb);
                    File.Delete($"assets/droppedCards/{cardid.Remove(0, 1)}1card.png");
                    File.Delete($"assets/droppedCards/{cardid.Remove(0, 1)}card.png");
                }
                if (interaction.Data.Name == "inventory")
                {
                    
                    bool privat = true;
                    try
                    {

                        if (interaction.Data.Options != null)
                        {
                            privat = Convert.ToBoolean(interaction.Data.Options.First().Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    await interaction.DeferAsync(ephemeral: privat);
                    Embed emb = UserCommands.Inventory(interaction.User);
                    ComponentBuilder builder = UserCommands.Inventory(interaction.User, true);
                    await interaction.FollowupAsync(ephemeral: privat, embed: emb, component: builder.Build(), options: null);
                }
                if (interaction.Data.Name == "view")
                {
                    await interaction.DeferAsync();
                    // Declaring up needed variables
                    string filename = "";
                    string idolName = "";
                    string group = "";
                    ulong owner = 0;
                    string rarity = "";
                    int internalRarity = 0;
                    // Getting the Cardid from the command
                    string cardid = interaction.Data.Options.First().Value.ToString();

                    // Removing the # if necessary
                    if (cardid.StartsWith("#"))
                    {
                        cardid = cardid.Remove(0, 1);
                    }

                    // Finding the card in the database and getting it's values
                    try
                    {
                        List<Card> specificCard = Database.UserInventory(cardid, true);
                        foreach (var item in specificCard)
                        {
                            filename = item.filename;
                            idolName = item.idol;
                            group = item.group;
                            owner = item.owner;
                            rarity = idgenerator.rarityTranslator(item.rarity, 0);
                            internalRarity = item.rarity;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        await interaction.FollowupAsync($"> {interaction.User.Mention} Hmm. I did not find your card in your inventory. Did you really give me the correct card id?", ephemeral: true);
                        return;
                    }

                    // Checking if author == owner of the card
                    SocketUser author = interaction.User;
                    if (owner != interaction.User.Id)
                    {
                        author = _client.GetUser(owner);
                    }
                    if (author is null)
                    {
                        await interaction.FollowupAsync($"> {interaction.User.Mention} Hmm. That is not your card.", ephemeral: true);
                        return;
                    }
                    // Building Image, Embed and Sending the message 
                    ImageProcessor.ImageBuilder(filename, idolName, group, rarity, internalRarity, cardid, "viewCards");
                    var emb = DropCommands.ViewEmbed(author, cardid, internalRarity, idolName, group);
                    await interaction.FollowupWithFileAsync(null, filePath: $"assets/viewCards/{cardid}1card.png", fileName: $"{cardid}1card.png", embed: emb);
                    File.Delete($"assets/viewCards/{cardid}1card.png");
                    File.Delete($"assets/viewCards/{cardid}card.png");

                }
                if (interaction.Data.Name == "profile")
                {
                    Embed emb = UserCommands.UserProfile(interaction.User);
                    await interaction.RespondAsync(embed: emb);
                }
                if (interaction.Data.Name == "sell")
                {
                    string cardid = interaction.Data.Options.FirstOrDefault().Value.ToString();
                    if (cardid.StartsWith("#"))
                    {
                        cardid = interaction.Data.Options.FirstOrDefault().Value.ToString().Remove(0,1);
                    }
                    UserCommands.Idol card = Database.FindCard(cardid);
                    if (card.exists == false)
                    {
                        await interaction.RespondAsync($"> ***I couldn't find a card with the Id of*** **`#{interaction.Data.Options.FirstOrDefault().Value.ToString()}`** \n > *Did you provide the **__correct__** Id?*", ephemeral: true);
                        return;
                    }
                    UserCommands.MemBase.SellCardValues.Remove(interaction.User.Id);
                    UserCommands.MemBase.SellCardValues.Add(interaction.User.Id, (cardid, card.internalRarity)); 
                    Embed emb = new EmbedBuilder()
                        .WithCurrentTimestamp()
                        .WithColor(idgenerator.rarityTranslator(card.internalRarity, 1))
                        .WithTitle("Are you sure?")
                        .WithDescription($"*You are about to sell:* \n > ➼ `{idgenerator.rarityTranslator(card.internalRarity, true)}` • **{card.idolName}** | {card.group} \n > That will bring you **`{idgenerator.GetRarityPrice(card.internalRarity)}`**🪙")
                        .Build();
                    await interaction.RespondAsync(null, embed: emb, component: ButtonHandler.AcceptAndDenyButtonsSell(1).Build());
                }
                if (interaction.Data.Name == "sellall")
                {

                    int internalRarity = Convert.ToInt32(interaction.Data.Options.FirstOrDefault().Value);
                    UserCommands.MemBase.SellAllCardList.Remove(interaction.User.Id);
                    var filteredUserInventory = Database.GetCardsWithRarity(interaction.User.Id, internalRarity);
                    UserCommands.MemBase.SellAllCardList.Add(interaction.User.Id, (filteredUserInventory));
                    List<string> embedTextInventory = new();
                    int price = 0;
                    foreach (var item in filteredUserInventory)
                    {
                        embedTextInventory.Add($"> ➼ **`#{item.id}`** • **{item.idol}** | *{item.group}*");
                        price += idgenerator.GetRarityPrice(item.rarity);
                    }
                    if (price == 0 || filteredUserInventory.Count == 0)
                    {
                        await interaction.RespondAsync("``` I didn't find any cards to sell with that rarity ```", ephemeral: true);
                        return;
                    }
                    Embed emb = new EmbedBuilder()
                        .WithCurrentTimestamp()
                        .WithColor(idgenerator.rarityTranslator(internalRarity, 1))
                        .WithTitle("Are you sure?")
                        .WithDescription($"*You are about to sell:* \n {String.Join("\n", embedTextInventory)} \n > That will bring you **`{price}`**🪙")
                        .Build();
                    await interaction.RespondAsync(null, embed: emb, component: ButtonHandler.AcceptAndDenyButtonsSell(2).Build());
                }
                if (interaction.Data.Name == "balance")
                {
                    int balance = Database.UserBalance(interaction.User.Id);
                    Embed emb = new EmbedBuilder()
                        .WithCurrentTimestamp()
                        .WithAuthor(interaction.User.Username)
                        .WithFooter("👑", iconUrl: interaction.User.GetAvatarUrl())
                        .WithColor(Discord.Color.Gold)
                        .WithTitle("Your personal bank account")
                        .WithDescription($"You currently have: \n \n > **`{balance}`** 🪙 \n > **`{Database.UserDiamonds(interaction.User.Id)}`** 💎 \n")
                        .Build();
                    await interaction.RespondAsync(null, embed: emb);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Interaction not found.");
                Console.WriteLine(ex);
            }
        }

        private async Task MyMessageComponentHandler(SocketMessageComponent interaction)
        {
            var customId = interaction.Data.CustomId;
            var user = (SocketGuildUser)interaction.User;
            var guild = user.Guild;

            // Forward Button (Inventory)
            if (interaction.Data.CustomId == "inventory_forward")
            {
                // Getting the inventory ranges and pages for the specific user
                int x = UserCommands.MemBase.InvValues[user.Id].x;
                int y = UserCommands.MemBase.InvValues[user.Id].y;
                int z = UserCommands.MemBase.InvValues[user.Id].z;

                // Getting embed and the button
                Embed Emb = ButtonHandler.InventoryForward(user);
                ComponentBuilder builder = ButtonHandler.InventoryForward(user, x, y, z);

                // Updating the embed
                await interaction.UpdateAsync(msg => { msg.Embed = Emb; msg.Components = builder.Build(); });
            }
            // Backward Button (Inventory)
            if (interaction.Data.CustomId == "inventory_back")
            {
                // Getting the inventory ranges and pages for the specific user
                int x = UserCommands.MemBase.InvValues[user.Id].x;
                int y = UserCommands.MemBase.InvValues[user.Id].y;
                int z = UserCommands.MemBase.InvValues[user.Id].z;

                // Getting embed and the button
                Embed Emb = ButtonHandler.InventoryBackwards(user);
                ComponentBuilder builder = ButtonHandler.InventoryBackwards(user, x, y, z);

                // Updating the embed
                await interaction.UpdateAsync(msg => { msg.Embed = Emb; msg.Components = builder.Build(); });
            }
            if (interaction.Data.CustomId == "1accept_button_sell")
            {
                
                
                int priceVariable = 0;
                string cardid = "";
                if (UserCommands.MemBase.SellCardValues.Where(x => x.Key ==interaction.User.Id).Count() == 1)
                { 
                    cardid = UserCommands.MemBase.SellCardValues[interaction.User.Id].cardid;
                    priceVariable = UserCommands.MemBase.SellCardValues[interaction.User.Id].price;
                }
                else
                {
                    await interaction.RespondAsync("*That is **__not your__** sell order!*", ephemeral: true);
                    return;
                }
                try
                {
                    Console.WriteLine(priceVariable);
                    UserCommands.SellCard(cardid, interaction.User.Id, priceVariable);
                    await interaction.Message.DeleteAsync();
                    await interaction.RespondAsync("**Sold the card.**", ephemeral: true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            if (interaction.Data.CustomId == "2accept_button_sell")
            {
                int price = 0;
                string cardid = "";
                List<Card> cardsToSellList = new();
                if (UserCommands.MemBase.SellAllCardList.Where(x => x.Key == interaction.User.Id).Count() == 1)
                {
                    cardsToSellList = UserCommands.MemBase.SellAllCardList[interaction.User.Id];
                }
                else
                {
                    await interaction.RespondAsync("*That is **__not your__** sell order!*", ephemeral: true);
                    return;
                }
                try
                {
                    foreach (var item in cardsToSellList)
                    {
                        UserCommands.SellCard(item.id, interaction.User.Id, item.rarity);
                        price += idgenerator.GetRarityPrice(item.rarity);
                    }
                    await interaction.Message.DeleteAsync();
                    await interaction.RespondAsync($"**Sold the cards for `{price}`🪙.**", ephemeral: true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    await interaction.Message.DeleteAsync();
                    await interaction.RespondAsync($"Something went wrong ... Can you try again?", ephemeral: true);
                }
            }
            if (interaction.Data.CustomId == "deny_button")
            {
                await interaction.Message.DeleteAsync();
                await interaction.RespondAsync("*I have **__canceled__** the action.*", ephemeral: true);
            }
        }
        public static async Task Drop()
        {
            List<SlashCommandBuilder> commands = SlashDropCommands.SlashCommands();
            foreach (var item in commands)
            {
                try
                {
                    await _client.Rest.CreateGlobalCommand(item.Build());
                }
                catch (ApplicationCommandException exception)
                {
                    var errorMessage = JsonConvert.SerializeObject(exception.Error, Formatting.Indented);
                    Console.WriteLine(errorMessage);
                }
            }
        }
    }
}
