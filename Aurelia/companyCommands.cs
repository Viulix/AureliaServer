using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;


namespace Aurelia
{
    public class companyCommands
    {
        public static Embed ViewCompany(SocketUser user)

        {
            List <Company> company = companies.GetCompany(user.Id);
            foreach (Company item in company)
            {
                Company comp = new Company
                {
                    id = item.id,
                    owner = item.owner,
                    name = item.name,
                    groups = item.groups,
                    producerClass = item.producerClass,
                    value = item.value,
                    rating = item.rating
                };
                var companyyEmbed = new EmbedBuilder()
                    .WithTitle("🏛️ " + comp.name)
                    .WithDescription("*Basic information about your company*")
                    .WithColor(Discord.Color.DarkTeal)
                    .WithTimestamp(DateTime.Now)
                    .AddField("Value:", $"> **`{comp.value}`** 🪙")
                    .AddField("Producer Class:", $"> **`{idgenerator.NumberInLetterTranslator(comp.producerClass)}`** ")
                    .AddField("Rating:", $"> **`{idgenerator.NumberInLetterTranslator(comp.rating)}`**");
                    
                if (comp.groups.Count == 0)
                {
                    companyyEmbed.AddField("Groups", "None");
                }
                else
                {
                    companyyEmbed.AddField("Groups:", $"> { String.Join("\n > ", comp.groups)}");
                }  
                return companyyEmbed.Build();
            }
            var companyEmbed = new EmbedBuilder()
                    .WithTitle("Whooops...")
                    .WithDescription("I couldn't find a company.")
                    .Build();
            return companyEmbed;
        }
        public static Embed AddGroup(SocketUser user, string name)
        {

            var emb = new EmbedBuilder()
                .WithAuthor(companies.GetCompanyName(user.Id))
                .WithTitle($"New group!")
                .WithDescription($"You successfully created a new group with the name `{name}`.")
                .Build();
            return emb;
        }
        public static Embed ShowGroupEmbed(SocketUser user, string name)
        {
            var emb = new EmbedBuilder()
                .WithFooter(user.Username, iconUrl: user.GetAvatarUrl())
                .WithColor(Discord.Color.DarkTeal)
                .WithTimestamp(DateTime.Now);
            var groupAsList = companies.GetGroup(name);
            foreach (var item in groupAsList)
            {
                List<string> groupIdols = new ();
                groupIdols.Add("***Members:*** \n");
                foreach (var id in item.idols)
                {
                    var card = Database.FindCard(id);
                    groupIdols.Add($"> **`#{id}`** • *{idgenerator.rarityTranslator(card.internalRarity, 0)}* • **{card.idolName}** | {card.group}");
                }
                string songs = "No songs released yet";
                if (item.songs.Count > 0) songs = String.Join("\n", item.songs);
                emb.WithAuthor("🏛️ " + item.company)
                .WithTitle(item.name)
                .WithDescription(String.Join("\n", groupIdols))
                .AddField("Streams", item.streams + " ")
                .AddField("Formed", item.DateOfCreation)
                .AddField("Songs", songs);

                return emb.Build();
            }
            return emb.Build();
        }
        public static Embed CreateSong(SocketUser user, string groupname, string songname)
        {
            try
                {
                var errorEmbed = new EmbedBuilder()
                    .WithTitle("Whoopsie... something went wrong!")
                    .WithColor(Discord.Color.DarkRed)
                    .WithTimestamp(DateTime.Now);
                if (companies.CheckIfCompanyExistsForUser(user.Id) == false)
                {
                    errorEmbed.WithDescription($"You do not have a company. Create one with `/create-company [name]`");
                    return errorEmbed.Build();
                }
                if (companies.IsGroupOwnedBy(groupname, user.Id) == false)
                {
                    errorEmbed.WithDescription($"The group you want to add an idol to is not owned by your company.");
                    return errorEmbed.Build();
                }
                var groups = companies.GetGroup(groupname);
                int songquality = 0;
                int choreoquality = 0;
                int length = 0;
                string groupName = "";
                foreach (var group in groups)
                {
                    groupName = group.name;
                    List<string> idols = group.idols;
                    length = idols.Count;
                    if (length <= 2)
                    {
                        errorEmbed.WithDescription($"This group is too small. At least three members have to be in it.");
                        return errorEmbed.Build();
                    }
                    if (group.SongCooldown + 86400 >= new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds())
                    {
                        long waitTime = (group.SongCooldown + 86400) - new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                        Console.WriteLine(waitTime);
                        int timeHours = Convert.ToInt32(waitTime / 3600);
                        int timeMinuts = Convert.ToInt32((waitTime / 60) % 60);
                        errorEmbed.WithDescription($"This group did just release a song. Please wait a bit longer until you release the next song! \n Please wait `{timeHours}h {timeMinuts}min`");
                        return errorEmbed.Build();
                    }
                    foreach (var item in idols)
                    {
                        var card = Database.FindCard(item);
                        songquality += card.voice;
                        choreoquality += card.dance;
                    }
                }
                string songid = idgenerator.CardIdGenerator(true, 1);
                DateTime releaseDate = DateTime.UtcNow.AddHours(3);
                int songquali = songquality / length + companies.GetProducerClass(user.Id) * 4;
                int chore = choreoquality / length;
                bool hit = false;
                Random rnd = new();

                if (rnd.Next(1, 101) > 98) hit = true;
                Song UserSong = new()
                {
                    Id = songid,
                    Owner = user.Id,
                    Group = groupName,
                    Name = songname,
                    Songquality = songquali,
                    Choreoquality = chore,
                    DailyStreams = 0,
                    TotalStreams = 0,
                    ReleaseDate = releaseDate,
                    Released = false,
                    Hit = hit
                };
                companies.SongCollection.InsertOne(UserSong);
                companies.AddSongToGroup(songname, songid);
                var emb = new EmbedBuilder()
                    .WithAuthor($"{groupName}")
                    .WithTitle($"{songname}!")
                    .WithColor(Discord.Color.DarkTeal)
                    .WithDescription($"{groupName} is producing a new song now. It is not released yet. New songs are being released at `22:00UTC` / `0:00 CEST`! \n \n **#`{songid}`** • \"{songname}\"")
                    .WithTimestamp(DateTime.Now);
                return emb.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
