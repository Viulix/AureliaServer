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
        private async Task MySlashCommandHandler(SocketSlashCommand command)
        {

            try
            {   
                int check = DropCommands.UserBasisCheck(command.User);
                switch (check)
                {
                    case 0:
                        break;
                    case 1:
                        Database.AddUser(command.User.Id);
                        break;
                }
                if (command.Data.Name == "drop")
                {
                    _ = command.DeferAsync();
                    if (check == 2)
                    {
                        await command.FollowupAsync("> **You have too many cards in your inventory. Please remove some before you drop again!**");
                        return;
                    }
                    long cooldown = Database.UserDropCooldown(command.User.Id);
                    var rn = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    if ((cooldown + 300) > rn && command.User.Id != 300570127869411329)
                    {
                        var waitTime = (cooldown + 300) - rn;
                        int timeMinuts = Convert.ToInt32((waitTime / 60));
                        var embe = new EmbedBuilder()
                        .WithColor(Discord.Color.DarkTeal)
                        .WithTimestamp(DateTime.Now)
                        .WithTitle("Not so fast!")
                        .WithDescription($"You cannot drop right now. Please wait `{timeMinuts}min`!")
                        .WithFooter(command.User.Username, iconUrl: command.User.GetAvatarUrl())
                        .Build();
                        await command.FollowupAsync(null, embed: embe);
                        return;
                    }
                    string cardid = DropCommands.randomIdoldInfo(command.User, 1);
                    int internalRarity = DropCommands.randomIdoldInfo(command.User, 2);
                    string rarity = DropCommands.randomIdoldInfo(command.User, 3);
                    List<string> idol = DropCommands.randomIdoldInfo(command.User, 4);
                    Embed emb = DropCommands.Drop(command.User, cardid.Remove(0, 1), internalRarity, idgenerator.rarityTranslator(internalRarity, 0), idol);
                    Database.AlbumAddCard(command.User.Id, idol[0], idol[2]);
                    Database.AddUserXpOrLevel(command.User.Id, internalRarity * 5);
                    Levelsystem.CheckUserLevelUp(command.User.Id);
                    _ = command.FollowupWithFileAsync(null,$"assets/droppedCards/{cardid.Remove(0, 1)}1card.png", fileName: $"{cardid.Remove(0, 1)}1card.png", embed: emb);
                    File.Delete($"assets/droppedCards/{cardid.Remove(0, 1)}1card.png");
                    File.Delete($"assets/droppedCards/{cardid.Remove(0, 1)}card.png");
                }
                if (command.Data.Name == "inventory")
                {
                    
                    bool privat = true;
                    try
                    {
                        if (command.Data != null)
                        {
                            privat = Convert.ToBoolean(command.Data.Options.FirstOrDefault().Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    await command.DeferAsync(ephemeral: privat);
                    Embed emb = UserCommands.Inventory(command.User);
                    ComponentBuilder builder = UserCommands.Inventory(command.User, true);
                    await command.FollowupAsync(ephemeral: privat, embed: emb, component: builder.Build(), options: null);
                }
                if (command.Data.Name == "view")
                {
                    await command.DeferAsync();
                    // Declaring up needed variables
                    string filename = "";
                    string idolName = "";
                    string group = "";
                    ulong owner = 0;
                    string rarity = "";
                    int internalRarity = 0;
                    // Getting the Cardid from the command
                    string cardid = command.Data.Options.FirstOrDefault().Value.ToString();

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
                        await command.FollowupAsync($"> {command.User.Mention} Hmm. I did not find your card in your inventory. Did you really give me the correct card id?", ephemeral: true);
                        return;
                    }

                    // Checking if author == owner of the card
                    SocketUser author = command.User;
                    if (owner != command.User.Id)
                    {
                        author = _client.GetUser(owner);
                    }
                    if (author is null)
                    {
                        await command.FollowupAsync($"> {command.User.Mention} Hmm. That is not your card.", ephemeral: true);
                        return;
                    }
                    // Building Image, Embed and Sending the message 
                    ImageProcessor.ImageBuilder(filename, idolName, group, rarity, internalRarity, cardid, "viewCards");
                    var emb = DropCommands.ViewEmbed(author, cardid, internalRarity, idolName, group);
                    await command.FollowupWithFileAsync(null, $"assets/viewCards/{cardid}1card.png", fileName: $"{cardid}1card.png", embed: emb);
                    File.Delete($"assets/viewCards/{cardid}1card.png");
                    File.Delete($"assets/viewCards/{cardid}card.png");
                    
                }
                if (command.Data.Name == "profile")
                {
                    Embed emb = UserCommands.UserProfile(command.User);
                    await command.RespondAsync(embed: emb);
                }
                if (command.Data.Name == "sell")
                {
                    string cardid = command.Data.Options.FirstOrDefault().Value.ToString();
                    if (cardid.StartsWith("#"))
                    {
                        cardid = command.Data.Options.FirstOrDefault().Value.ToString().Remove(0,1);
                    }
                    UserCommands.Idol card = Database.FindCard(cardid);
                    if (card.exists == false)
                    {
                        await command.RespondAsync($"> ***I couldn't find a card with the Id of*** **`#{command.Data.Options.FirstOrDefault().Value.ToString()}`** \n > *Did you provide the **__correct__** Id?*", ephemeral: true);
                        return;
                    }
                    UserCommands.MemBase.SellCardValues.Remove(command.User.Id);
                    UserCommands.MemBase.SellCardValues.Add(command.User.Id, (cardid, card.internalRarity)); 
                    Embed emb = new EmbedBuilder()
                        .WithCurrentTimestamp()
                        .WithColor(idgenerator.rarityTranslator(card.internalRarity, 1))
                        .WithTitle("Are you sure?")
                        .WithDescription($"*You are about to sell:* \n > ➼ `{idgenerator.rarityTranslator(card.internalRarity, true)}` • **{card.idolName}** | {card.group} \n > That will bring you **`{idgenerator.GetRarityPrice(card.internalRarity)}`**🪙")
                        .Build();
                    await command.RespondAsync(null, embed: emb, component: ButtonHandler.AcceptAndDenyButtonsSell(1).Build(), ephemeral: true);
                }
                if (command.Data.Name == "sellall")
                {

                    int internalRarity = Convert.ToInt32(command.Data.Options.FirstOrDefault().Value);
                    UserCommands.MemBase.SellAllCardList.Remove(command.User.Id);
                    var filteredUserInventory = Database.GetCardsWithRarity(command.User.Id, internalRarity);
                    UserCommands.MemBase.SellAllCardList.Add(command.User.Id, (filteredUserInventory));
                    List<string> embedTextInventory = new();
                    int price = 0;
                    foreach (var item in filteredUserInventory)
                    {
                        embedTextInventory.Add($"> ➼ **`#{item.id}`** • **{item.idol}** | *{item.group}*");
                        price += idgenerator.GetRarityPrice(item.rarity);
                    }
                    if (price == 0 || filteredUserInventory.Count == 0)
                    {
                        await command.RespondAsync("``` I didn't find any cards to sell with that rarity ```", ephemeral: true);
                        return;
                    }
                    Embed emb = new EmbedBuilder()
                        .WithCurrentTimestamp()
                        .WithColor(idgenerator.rarityTranslator(internalRarity, 1))
                        .WithTitle("Are you sure?")
                        .WithDescription($"*You are about to sell:* \n {String.Join("\n", embedTextInventory)} \n > That will bring you **`{price}`**🪙")
                        .Build();
                    await command.RespondAsync(null, embed: emb, component: ButtonHandler.AcceptAndDenyButtonsSell(2).Build(), ephemeral: true);
                }
                if (command.Data.Name == "balance")
                {
                    int balance = Database.UserBalance(command.User.Id);
                    Embed emb = new EmbedBuilder()
                        .WithCurrentTimestamp()
                        .WithAuthor(command.User.Username)
                        .WithFooter("👑", iconUrl: command.User.GetAvatarUrl())
                        .WithColor(Discord.Color.Gold)
                        .WithTitle("Your personal bank account")
                        .WithDescription($"You currently have: \n \n > **`{balance}`** 🪙 \n > **`{Database.UserDiamonds(command.User.Id)}`** 💎 \n")
                        .Build();
                    await command.RespondAsync(null, embed: emb);
                }
                if (command.Data.Name == "daily")
                {
                    var emb = UserCommands.DailyDrop(command.User.Id);
                    await command.DeferAsync();
                    await command.FollowupAsync(null, embed: emb);
                    if (Levelsystem.CheckUserLevelUp(command.User.Id) == true)
                    {
                        await command.FollowupAsync($"{command.User.Mention} you just leveled up to** level {Database.UserLevel(command.User.Id, 0)}**!", ephemeral: true);
                    }

                }
                if (command.Data.Name == "album")
                {
                    string group = (string)command.Data.Options.FirstOrDefault().Value;
                    Console.WriteLine(group);
                    var emb = UserCommands.UserAlbums(command.User, group);
                    await command.RespondAsync(null, embed: emb);
                }
                if (command.Data.Name == "create")
                {
                    string fieldName = command.Data.Options.First().Name;
                    switch (fieldName)
                    {
                        case "group":
                            Console.WriteLine("group");
                            string groupName = command.Data.Options.First().Options.First().Value.ToString();
                            var groupEmbed = companies.CreateGroup(groupName, command.User.Id);
                            await command.RespondAsync(null, embed: groupEmbed);
                            break;
                        case "company":
                            Console.WriteLine("company");
                            string companyName = command.Data.Options.First().Options.First().Value.ToString();
                            Console.WriteLine(companyName);
                            await command.RespondAsync(null, embed: companies.CreateCompany(command.User.Id, companyName));
                            break;
                        case "song":
                            string songname = command.Data.Options.First().Options.Last().Value.ToString();
                            string groupname = command.Data.Options.First().Options.First().Value.ToString();
                            var emb = companyCommands.CreateSong(command.User, groupname, songname);
                            await command.RespondAsync(null, embed: emb);
                            break;
                        default:
                            break;
                    }
                }
                if (command.Data.Name == "mycompany")
                {
                    var emb = companyCommands.ViewCompany(command.User);
                    await command.RespondAsync(null, embed: emb);
                }
                if (command.Data.Name == "addidol")
                {
                    string groupname = (string)command.Data.Options.First();
                    string cardid = (string)command.Data.Options.Last();
                    var emb = companies.AddIdol(cardid, command.User.Id, groupname);
                    await command.RespondAsync(null, embed: emb);
                }
                if (command.Data.Name == "showgroup")
                {
                    string groupname = (string)command.Data.Options.First().Value;
                    var emb = companyCommands.ShowGroupEmbed(command.User, groupname);
                    await command.RespondAsync(null, embed: emb);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Interaction not found or failed to run.");
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
                    if (cardsToSellList.Count == 0)
                    {
                        await interaction.RespondAsync($"You already sold these cards.?", ephemeral: true);
                        return;
                    }
                    foreach (var item in cardsToSellList)
                    {
                        UserCommands.SellCard(item.id, interaction.User.Id, item.rarity);
                        price += idgenerator.GetRarityPrice(item.rarity);
                    }
                    UserCommands.MemBase.SellAllCardList[interaction.User.Id].Clear();
                    await interaction.RespondAsync($"**Sold the cards for `{price}`🪙.**", ephemeral: true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
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
