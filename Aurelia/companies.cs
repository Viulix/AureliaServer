using Discord;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurelia
{
    class companies
    {
        static readonly IMongoDatabase db = Database.Db;
        static readonly IMongoCollection<Company> CompanyCollection = db.GetCollection<Company>("companies");
        static readonly IMongoCollection<Group> GroupCollection = db.GetCollection<Group>("groups");
        static public readonly IMongoCollection<Song> SongCollection = db.GetCollection<Song>("songs");
        public static Embed CreateCompany(ulong userid, string name)
        {
            if (CheckCompanyName(name) is false)
            {
                var embeed = new EmbedBuilder()
                    .WithTitle("There is already a company with that name.")
                    .WithColor(Discord.Color.DarkRed)
                    .Build();
                return embeed;
            }
            var result = CompanyCollection.Find(x => x.owner == userid).ToList();
            if (result.Count != 0)
            {
                var embd = new EmbedBuilder()
                    .WithTitle("You already have a company. You cannot have multiple ones.")
                    .WithColor(Discord.Color.DarkRed)
                    .Build();
                return embd;
            };
            string companyid = NewCompanyId(0);
            var company = new Company
            {
                id = companyid,
                owner = userid,
                name = name,
                groups = new List<string>() { },
                producerClass = 1,
                value = 100,
                rating = 1
            };
            CompanyCollection.InsertOne(company);
            var embed = new EmbedBuilder()
                .WithTitle("Success!")
                .WithDescription($"You just founded a new company witht the name **`{name}`**")
                .WithColor(Discord.Color.DarkRed)
                .Build();
            return embed;
        }
        public static string NewCompanyId(int identifier)
        {
            bool check = true;
            string companyid = "";
            while (check is true)
            { 
                StringBuilder builder = new StringBuilder();
                Enumerable
                   .Range(65, 26)
                    .Select(e => ((char)e).ToString())
                    .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                    .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                    .OrderBy(e => Guid.NewGuid())
                    .Take(16)
                    .ToList().ForEach(e => builder.Append(e));
                    companyid = builder.ToString().ToUpper();
                if (identifier == 0)
                {
                    check = CheckCompanyId(companyid);
                }
                if (identifier == 1)
                {
                    check = CheckGroupId(companyid);
                }

            }
            return companyid; 
        }
        public static bool CheckCompanyId(string companyid)
        {
            var check = CompanyCollection.Find(x => x.id == companyid).ToList();
            if (check.Count == 0) return false;
            else return true;
        }
        public static bool CheckCompanyName(string companyname)
        {
            var check = CompanyCollection.Find(x => x.id == companyname).ToList();
            if (check.Count == 0) return true;
            else return false;
        }
        public static bool IsGroupOwnedBy(string groupname, ulong userid)
        {
            var check = GroupCollection.Find(x => x.name == groupname).ToList();
            foreach (var item in check)
            {
                if (item.owner == userid) return true;
            }
            return false;
        }
        public static List<Company> GetCompany(ulong userid)
        {
            List<Company> check = CompanyCollection.Find(x => x.owner == userid).Limit(1).ToList();
            return check;
        }
        public static List<Group> GetGroup(string name)
        {
            List<Group> check = GroupCollection.Find(x => x.name == name).Limit(1).ToList();
            if (check.Count == 0) return null;
            return check;
        }
        public static int GetGroupPopularity(string name)
        {
            List<Group> check = GroupCollection.Find(x => x.name == name).Limit(1).ToList();
            if (check.Count == 0) return 0;
            int pop = 0;
            foreach (var item in check)
            {
                pop = item.popularity;
            }
            return pop;
        }
        public static string GetCompanyName(ulong userid)
        {
            List<Company> check = CompanyCollection.Find(x => x.owner == userid).Limit(1).ToList();
            string companyName = "";
            foreach (var item in check)
            {
                companyName = item.name;
            }
            return companyName;
        }
        public static Embed CreateGroup(string name, ulong user)
        {
            if (CheckIfCompanyExistsForUser(user) == false)
            {
                var errorEmbed = new EmbedBuilder()
                    .WithTitle("That did not work...")
                    .WithDescription($"You do not have a company. Create one with `/create-company [name]`")
                    .Build();
                return errorEmbed;
            }
            if (CheckGroupName(name) == false)
            {
                var errorEmbed2 = new EmbedBuilder()
                    .WithTitle("That did not work...")
                    .WithDescription($"There is already a group with the name `{name}`")
                    .Build();
                return errorEmbed2;
            }
            else 
            {
                Group newGroup = new()
                {
                    id = NewCompanyId(1),
                    sizelimit = 4,
                    owner = user,
                    name = name,
                    company = GetCompanyName(user),
                    idols = new List<string>() { },
                    streams = 0,
                    songs = new List<string>() { },
                    DateOfCreation = DateTime.Now
                };
                GroupCollection.InsertOne(newGroup);
                var company = CompanyCollection.Find(x => x.owner == user).ToList();
                foreach (var item in company)
                {
                    if (item.groups.Count <= 3 && item.owner == user)
                    {
                        CompanyCollection.FindOneAndUpdate(y => y.owner == user, Builders<Company>.Update.Push(x => x.groups, newGroup.name));
                        var successEmbed = new EmbedBuilder()
                            .WithTitle($"Welcome {name}!")
                            .WithDescription($"You successfully created a new group with the name `{name}`. \n Use `/addidol` to add idols to this group!")
                            .WithTimestamp(DateTime.Now)
                            .Build();
                        return successEmbed;
                    }
                    else
                    {
                        string desc = $"You have too many groups. You can only have 3 groups!";
                        if (item.owner != user) desc = "It seems like this is not your group / company! See your groups with `/show groups`";
                        var errorEmbed3 = new EmbedBuilder()
                            .WithTitle("That did not work...")
                            .WithDescription(desc)
                            .Build();
                        return errorEmbed3;
                    }
                }
                return null;
            }
        }
        public static Embed AddIdol(string cardid, ulong user, string groupName)
        {
            var errorEmbed = new EmbedBuilder()
                .WithTitle("That did not work...")
                .WithColor(Discord.Color.DarkRed)
                .WithTimestamp(DateTime.Now);

            if (CheckIfCompanyExistsForUser(user) == false)
            {
                errorEmbed.WithDescription($"You do not have a company. Create one with `/create-company [name]`");
                return errorEmbed.Build();
            }
            if (IsGroupOwnedBy(groupName, user) == false)
            {
                errorEmbed.WithDescription($"The group you want to add an idol to is not owned by your company.");
                return errorEmbed.Build();
            }
            if (cardid.StartsWith('#'))
            {
                cardid = cardid.Remove(0, 1);
            }
            var card = Database.FindCard(cardid);
            if (card.exists == false || card.owner != user)
            {
                errorEmbed.WithDescription($"That is either not your card or this card does not exists!");
                return errorEmbed.Build();
            }
            var group = GroupCollection.Find(x => x.name == groupName).ToList();
            foreach (var item in group)
            {
                if (item.idols.Count <= item.sizelimit)
                {
                    int groupPop = 0;
                    foreach (var idol in item.idols)
                    {
                        var thisCard = Database.FindCard(idol);
                        if (thisCard.idolName == card.idolName && thisCard.group == card.group)
                        {
                            errorEmbed.WithDescription("You already have that idol in your group. Keep in mind that no matter how many cards you have from one idol, you can place it only once in a group!");
                            errorEmbed.AddField("Idol in group", $"> **{thisCard.idolName}** | {thisCard.group}", true);
                            errorEmbed.AddField("Idol you want to add", $"> **{card.idolName}** | {card.group}", true);
                            return errorEmbed.Build();
                        }
                        groupPop += thisCard.popularity;
                    }
                    
                    var successEmbed = new EmbedBuilder()
                        .WithTitle($"A new member joined {item.name}!")
                        .WithColor(Discord.Color.Green)
                        .WithDescription($"{card.idolName} did successfully join {item.name}!")
                        .WithTimestamp(DateTime.Now)
                        .Build();
                    int groupLength = 1;
                    if (item.idols.Count > 0) groupLength = item.idols.Count;

                    GroupCollection.FindOneAndUpdate(y => y.owner == user, Builders<Group>.Update.Push(x => x.idols, cardid).Set(x => x.popularity, groupPop / groupLength));
                    Database.RemoveFromInventoryOnly(cardid, user);
                    return successEmbed;
                }
                else
                {
                    errorEmbed.WithDescription($"You have too many idols in that group. You can only have {item.sizelimit} groups!");
                    return errorEmbed.Build();
                }
            }
            return null;
        }
        public static bool CheckGroupName(string groupname)
        {
            var check = GroupCollection.Find(x => x.name == groupname).ToList();
            if (check.Count == 0) return true;
            else return false;
        }
        public static bool CheckGroupNameInCompany(ulong user, string groupname)
        {
            var check = GroupCollection.Find(x => x.owner == user).ToList();
            if (check.Count == 0) return false;
            else return true;
        }
        public static bool CheckGroupId(string newid)
        {
            var check = GroupCollection.Find(x => x.id == newid).ToList();
            if (check.Count == 0) return false;
            else return true;
        }
        public static bool CheckIfCompanyExistsForUser(ulong userid)
        {
            List<Company> check = CompanyCollection.Find(x => x.owner == userid).Limit(1).ToList();
            if (check.Count == 0) return false;
            else return true;
        }
        public static bool IsSongNameAvailable(string name)
        {
            List<Song> check = SongCollection.Find(x => x.Name == name).Limit(1).ToList();
            if (check.Count == 0) return false;
            else return true;
        }
        public static bool IsSongIdAvailable(string Id)
        {
            List<Song> check = SongCollection.Find(x => x.Id == Id).Limit(1).ToList();
            if (check.Count == 0) return false;
            else return true;
        }
        public static int GetProducerClass(ulong userid)
        {
            List<Company> check = CompanyCollection.Find(x => x.owner == userid).Limit(1).ToList();
            int producerClass = 0;
            foreach (var item in check)
            {
                producerClass = item.producerClass;
            }
            return producerClass;
        }
        public static void AddSongToGroup(string songname, string songid)
        {
            try
            {
                var update = Builders<Group>.Update.Push<string>(e => e.songs, songid).Set(x => x.SongCooldown, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());
                GroupCollection.FindOneAndUpdate(x => x.name == songname, update);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static void SetSongToPublished()
        {
            DateTime now = DateTime.UtcNow;
            var result = SongCollection.Find(x => x.Released == false && x.ReleaseDate <= now).ToList();
            foreach (var item in result)
            {
                Console.WriteLine(item.ReleaseDate);
                var update = Builders<Song>.Update.Set(x => x.Released, true);
                SongCollection.FindOneAndUpdate(x => x.Id == item.Id, update);
            }
        }
    }
}
 