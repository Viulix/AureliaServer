using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Aurelia
{
    public class Database
    {
        static IMongoClient client = new MongoClient("");
        static IMongoDatabase db = client.GetDatabase("Aurelia");
        static IMongoCollection<User> userCollection = db.GetCollection<User>("user");
        static IMongoCollection<Card> cardidCollection = db.GetCollection<Card>("cardids");
        static IMongoCollection<Idol> idolCollection = db.GetCollection<Idol>("idols");
        public static void AddUser(ulong userid)
        {
            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var user = new User
            {
                id = userid,
                dailystamp = 0,
                joined = Timestamp,
                lvl = 1,
                xp = 0,
                balance = 500,
                diamonds = 10,
                luck = 0,
                inventory = new List<string>(),
                albums = new Album
                {
                    BLACKPINK = new BLACKPINK()
                    {
                        Complete = 0,
                        Lisa = 0,
                        Jennie = 0,
                        Rose = 0,
                        Jisoo = 0
                    },
                    TWICE = new TWICE()
                    {
                        Complete = 0,
                        Sana = 0,
                        Nayeon = 0,
                        Momo = 0,
                        Mina = 0,
                        Chaeyoung = 0,
                        Dahyun = 0,
                        Jihyo = 0,
                        Tzuyu = 0,
                        Jeongyeon = 0,
                    }
                }
            };
            userCollection.InsertOne(user);
        }
        public static bool CheckUser(ulong userid)
        {
            var check = userCollection.Find(x => x.id == userid).ToList(); ;
            if (check.Count == 0) return false;
            else return true;
        }
        public static bool CheckUserInventoryLength(ulong userid)
        {
            var users = userCollection.Find(x => x.id == userid && x.inventory.Count < 100).ToList();
            if (users.Count == 0) return false;
            else return true;
        }
        public static List<string> UserInventory(ulong userid, byte format, int x, int y)
        {
            var results = userCollection.Find(p => p.id == userid).ToList();

            List<string> inv = new List<string>();
            List<string> resultList = new List<string>();

            foreach (var item in results)
            {
                for (int i = x; i < item.inventory.Count && i < y; i++)
                {
                    inv.Add(item.inventory[i]);
                }
            }
            for (int i = 0; i < inv.Count; i++)
            {
                resultList.Add($"➼ **`#{inv[i]}`** • {Database.UserInventory(inv[i])}");
            }
            return resultList;
        }
        public static int UserInventoryLength(ulong userid)
        {
            var results = userCollection.Find(p => p.id == userid).ToList();
            List<string> inv = new List<string>();
            foreach (var item in results)
            {
                for (int i = 0; i < item.inventory.Count; i++)
                {
                    inv.Add(item.inventory[i]);
                }
            }
            return inv.Count;
        }
        public static List<string> UserInventory(ulong userid, int x, int y)
        {
            var results = userCollection.Find(p => p.id == userid).ToList();

            List<string> inv = new List<string>();

            foreach (var item in results)
            {
                for (int i = x; i < item.inventory.Count && i < y; i++)
                {
                    inv.Add(item.inventory[i]);
                }
            }
            return inv;
        }
        public static string UserInventory(string cardid)
        {
            var results = cardidCollection.Find(p => p.id == cardid).ToList();
            string result = "";
            byte em = 0;
            try
            {
                foreach (var item in results)
                {
                    result = ($"`{idgenerator.rarityTranslator(item.rarity, em)}` •  **{item.idol}**" + " | " + $"*{item.group}*");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return result;
        }
        public static int UserBalance(ulong userid)
        {
            int balance = 0;
            var user = userCollection.Find(x => x.id == userid).Limit(1).ToList();
            foreach (var item in user)
            {
                balance = item.balance;
            }
            return balance;
        }
        public static int UserDiamonds(ulong userid)
        {
            int diamonds = 0;
            var user = userCollection.Find(x => x.id == userid).Limit(1).ToList();
            foreach (var item in user)
            {
                diamonds = item.diamonds;
            }
            return diamonds;
        }
        public static UserCommands.Idol FindCard(string cardid)
        {
            UserCommands.Idol idol = new UserCommands.Idol { }; 
            var results = cardidCollection.Find(p => p.id == cardid).ToList();
            if (results == null)
            {
                idol.exists = false;
                return idol;
            }
            try
            {
                foreach (var item in results)
                {
                    idol.exists = true;
                    idol.idolName = item.idol;
                    idol.group = item.group;
                    idol.internalRarity = item.rarity;
                    idol.owner = item.owner;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return idol;
        }
        public static int UserLevel(ulong userid, int indicator)
        {
            int level = 0;
            int xp = 0;
            List<User> result = userCollection.Find(x => x.id == userid).ToList();
            foreach (User item in result)
            {
                level = item.lvl;
                xp = item.xp;
            }
            if (indicator == 1) return xp;
            else return level;
        }
        public static long UserDailyStamp(ulong userid)
        {
            var results = userCollection.Find(p => p.id == userid).ToList();
            long timestamp = 0;
            foreach (var item in results)
            {
                timestamp = item.dailystamp;     
            }
            return timestamp;
        }
        public static void UserUpdateDailyStamp(ulong userid)
        {
            try
            {
                var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                var update = Builders<User>.Update.Set<long>(e => e.dailystamp, timestamp);
                userCollection.FindOneAndUpdate(x => x.id == userid, update);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static List<Card> UserInventory(string cardid, bool yee)
        {
            var results = cardidCollection.Find(p => p.id == cardid).ToList();

            return results;
        }
        public static List<Card> GetCardsWithRarity(ulong userid, int rarity)
        {
            var results = userCollection.Find(p => p.id == userid).ToList();
            List<string> inv = new();
            List<Card> cards = new();
            foreach (var item in results)
            {
                for (int i = 0; i < item.inventory.Count; i++)
                {
                    inv.Add(item.inventory[i]);
                }
            }
            cards = cardidCollection.Find(p => p.owner == userid && p.rarity == rarity).Limit(15).ToList();
            return cards;
        }
        public static void AddCardToInventory(ulong userid, string cardid)
        {
            try
            {
                var update = Builders<User>.Update.Push<string>(e => e.inventory, cardid);
                userCollection.FindOneAndUpdate(x => x.id == userid, update);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static void AddUserXpOrLevel(ulong userid, int xp)
        {
            try
            {
                var update = Builders<User>.Update.Inc<int>(e => e.xp, xp);
                userCollection.FindOneAndUpdate(x => x.id == userid, update);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static void AddUserDiamonds(ulong userid, int diamondAmount)
        {
            try
            {
                var update = Builders<User>.Update.Inc<int>(e => e.diamonds, diamondAmount);
                userCollection.FindOneAndUpdate(x => x.id == userid, update);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static void AddUserLevel(ulong userid, int lvl)
        {
            try
            {
                var update = Builders<User>.Update.Inc<int>(e => e.lvl, lvl);
                userCollection.FindOneAndUpdate(x => x.id == userid, update);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static List<object> GetUserGroupAlbum(ulong userid, string group)
        {

            try
            {
                List<User> result = userCollection.Find(x => x.id == userid).ToList();
                List<object> finalResult = new();
                StringBuilder sb = new();
                foreach (User item in result)
                {
                    var groupObj = item.albums.GetType().GetProperty(group).GetValue(item.albums);
                    finalResult.Add(groupObj);
                }
                return finalResult;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        public static void AlbumAddCard(ulong userid, string IdolName, string group)
        {

            try
            {
                var update = Builders<User>.Update.Inc<Album>(e => e.albums, new Album());
                List<User> result = userCollection.Find(x => x.id == userid).ToList();
                List<User> checkResult = userCollection.Find(x => x.id == userid).ToList();
                foreach (User item in result)
                {
                    var groupObj = item.albums.GetType().GetProperty(group).GetValue(item.albums);
                    var check = groupObj.GetType().GetProperty(IdolName);
                    var value = Convert.ToInt32(check.GetValue(groupObj));
                    if (value == 0)
                    {
                        check.SetValue(groupObj, 1);
                    }
                    bool proof = true;
                    foreach (var idol in groupObj.GetType().GetProperties())
                    {
                        var it = Convert.ToInt32(idol.GetValue(groupObj));
                        Console.WriteLine(it);
                        if (it == 0 && idol.Name != "Complete")
                        {
                            proof = false;
                        }
                    }
                    var completedCheck = groupObj.GetType().GetProperty("Complete");
                    var value2 = Convert.ToInt32(completedCheck.GetValue(groupObj));
                    if (value2 == 0 && proof is true)
                    {
                        completedCheck.SetValue(groupObj, 1);
                    }
                    update = Builders<User>.Update.Set<Album>(e => e.albums, item.albums);
                }
                userCollection.FindOneAndUpdate(x => x.id == userid, update);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static bool CheckCardId(string cardid)
        {
            var check = cardidCollection.Find(x => x.id == cardid).ToList();
            if (check.Count == 0) return false;
            else return true;
        }
        public static void RemoveCard(string cardid, ulong userid)
        {
            var check = cardidCollection.Find(x => x.id == cardid).ToList();
            if (check.Count == 0) return;
            else
            {
                try
                {
                    var update = Builders<User>.Update.Pull<string>(e => e.inventory, cardid);
                    userCollection.FindOneAndUpdate(x => x.id == userid, update);
                    cardidCollection.FindOneAndDelete(c => c.id == cardid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        public static void InsertCard(string cardid, string filename, string idol, string group, string era, int rarity, ulong userid, int shiny)
        {
            Card newCard = new Card
            {
                id = cardid,
                owner = userid,
                filename = filename,
                idol = idol,
                group = group,
                era = era,
                rarity = rarity,
                shiny = shiny
            };
            try
            {
                cardidCollection.InsertOne(newCard);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static dynamic RandomIdol()
        {
            List<string> idol = new List<string>();
            var count = idolCollection.EstimatedDocumentCount();
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            int rndNumber = rnd.Next(1, Convert.ToInt32(count) + 1);
            var rgCard = idolCollection.Find(x => x._id == rndNumber)
                .Limit(1)
                .ToList();
            foreach (var rndCard in rgCard)
            {
                idol.Add(rndCard.idol); idol.Add(rndCard.filename); idol.Add(rndCard.group);idol.Add(rndCard.era);
            }
            return idol;
        }
        public static void AddMoneyToUser(ulong userid, int amount)
        {
            var update = Builders<User>.Update.Inc(x => x.balance, amount);
            userCollection.FindOneAndUpdate(x => x.id == userid, update);
        }
    }
}
