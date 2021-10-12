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
        static IMongoClient client = new MongoClient("mongodb://viu:BaXtEr89@192.168.178.41:27017/?authSource=admin&readPreference=primary&appname=MongoDB%20Compass&directConnection=true&ssl=false");
        static IMongoDatabase db = client.GetDatabase("Aurelia");
        static IMongoCollection<Company> companyCollection = db.GetCollection<Company>("companies");
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
            var result = companyCollection.Find(x => x.owner == userid).ToList();
            if (result.Count != 0)
            {
                var embd = new EmbedBuilder()
                    .WithTitle("You already have a company. You cannot have multiple ones.")
                    .WithColor(Discord.Color.DarkRed)
                    .Build();
                return embd;
            };
            string companyid = NewCompanyId();
            var company = new Company
            {
                id = companyid,
                owner = userid,
                name = name,
                groups = new List<List<string>>() { },
                producerClass = 1,
                value = 100,
                rating = 1
            };
            companyCollection.InsertOne(company);
            var embed = new EmbedBuilder()
                .WithTitle("Success!")
                .WithDescription($"You just founded a new company witht the name **`{name}`**")
                .WithColor(Discord.Color.DarkRed)
                .Build();
            return embed;
        }
        public static string NewCompanyId()
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
                    check = CheckCompanyId(companyid);
            }
            return companyid; 
        }
        public static bool CheckCompanyId(string companyid)
        {
            var check = companyCollection.Find(x => x.id == companyid).ToList();
            if (check.Count == 0) return false;
            else return true;
        }
        public static bool CheckCompanyName(string companyname)
        {
            var check = companyCollection.Find(x => x.id == companyname).ToList();
            if (check.Count == 0) return true;
            else return false;
        }
    }
}
 