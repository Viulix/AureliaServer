using Discord;
using Discord.WebSocket;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;

namespace Aurelia
{
    public class DropCommands
    {

        public static Embed Drop(Discord.WebSocket.SocketUser user, string cardid, int internalRarity, string rarity, List<string> idol)
        {

            try
            {
                //Embed
                ImageProcessor.ImageBuilder(idol[1], idol[0], idol[2], rarity, internalRarity, cardid, "droppedCards");
                Random rnd = new();
                int rnd1 = internalRarity * rnd.Next(10, 21);
                int rnd2 = internalRarity * rnd.Next(10, 21);
                int rnd3 = internalRarity * rnd.Next(10, 21);
                Database.InsertCard(cardid, idol[1], idol[0], idol[2], idol[3], internalRarity, user.Id, 0, rnd1, rnd2, rnd3);
                Database.AddCardToInventory(user.Id, cardid);
                var emb = new EmbedBuilder()
                    .WithColor(idgenerator.rarityTranslator(internalRarity, 1))
                    .WithTimestamp(DateTime.Now)
                    .WithTitle($"✨ {user.Username} dropped a card!")
                    .WithDescription($"**👤Idol:** \n >>> `[{idgenerator.rarityTranslator(internalRarity, true)}]` \n **{idol[0]}**  | ***{idol[2]}*** \n **👤__Stats:__** \n \n 🎙️ Voice: {rnd3} \n 💃 Dance: {rnd2} \n 🗣️ Popularity: {rnd1}")
                    .AddField("📋Id:", $">>> **`#{cardid}`**")
                    .WithImageUrl($"attachment://{cardid}1card.png")
                    .WithFooter(user.Username, iconUrl: user.GetAvatarUrl())
                    .Build();
                var update = Builders<User>.Update.Set<long>(x => x.dropCooldown, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());
                Database.UserCollection.FindOneAndUpdate(y => y.id == user.Id, update);
                return emb;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        public static dynamic randomIdoldInfo(Discord.WebSocket.SocketUser user, int specifisier)
        {

            string cardid = idgenerator.CardIdGenerator(true, 0);
            int internalRarity = idgenerator.raritySetter();
            string rarity = idgenerator.rarityTranslator(internalRarity, 0);
            var idol = Database.RandomIdol();
            

            switch (specifisier)
            {
                case 1:
                    return cardid;
                case 2:
                    return internalRarity;
                case 3:
                    return rarity;
                case 4:
                    return idol;
                default:
                    return null; ;
            }            
        }
        public static int UserBasisCheck(Discord.WebSocket.SocketUser user)
        {
            bool result = Database.CheckUser(user.Id);
            bool result2 = Database.CheckUserInventoryLength(user.Id);

            if (result is false) return 1;
            if (result2 is false) return 2;
            else return 0;
        }
        public static Embed ViewEmbed(SocketUser user, string cardid, int internalRarity, string idolName, string idolGroup)
        {

            try
            {
                var emb = new EmbedBuilder()
                    .WithColor(idgenerator.rarityTranslator(internalRarity, 1))
                    .WithTimestamp(DateTime.Now)
                    .WithTitle($"📖  {user.Username}'s card!")
                    .WithDescription($"**👤Idol:** \n >>> `[{idgenerator.rarityTranslator(internalRarity, true)}]` | **{idolName}**  | ***{idolGroup}***")
                    .AddField("📋Id:", $">>> **`#{cardid}`**")
                    .WithImageUrl($"attachment://{cardid}1card.png")
                    .WithFooter(user.Username, iconUrl: user.GetAvatarUrl())
                    .Build();
                    return emb;
            }
            catch(Exception ex)
            {   
                return null;
            }
            
        }
    }
}
