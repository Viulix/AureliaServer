using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading;



namespace Aurelia
{
    class DailyUpdates
    {
        public static void DailyUpdate()
        {
            Console.WriteLine($"{DateTime.Now:T} Checking time...");
            var dateNow = DateTime.Now;
            var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day, 0, 0, 0);
            var differencee = date - dateNow;
            var difference = differencee.TotalMilliseconds;
            if (difference < 0)
            {
                difference += 86400000;
            }
            Console.WriteLine($"{DateTime.Now:T} Next update will be at {date:HH:mm:ss}");
            var milisecondsInDay = TimeSpan.FromDays(1).TotalMilliseconds;
            var timer = new Timer(RunThisEveryDay, null, (int)difference, (int)milisecondsInDay);

            Thread.Sleep(Timeout.Infinite);
        }
        private static void RunThisEveryDay(object state)
        {
            var allsongs = companies.SongCollection.Find(Builders<Song>.Filter.Where(x => x.Released == true)).ToList();
            foreach (var song in allsongs)  
            {
                Console.WriteLine($"{DateTime.Now:T} Calculating streams for song: {song.Name} | {song.Id}");
                int chorequali = song.Choreoquality;
                int songquali = song.Songquality;
                int daysSinceRelease = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal((DateTime.UtcNow - song.ReleaseDate).TotalDays)));
                int groupPopularity = companies.GetGroupPopularity(song.Group);
                int hitFactor = 1;
                if (song.Hit == true) hitFactor = 2;
                Random rnd = new Random();
                int streams = ((rnd.Next(1000, 2001) * (chorequali + songquali) + 10000 * groupPopularity) * hitFactor) - daysSinceRelease * rnd.Next(3000, 5001);
                Console.WriteLine($"{DateTime.Now:T} Calculating streams for song: {chorequali + " + " +  songquali} | Popularity: {groupPopularity}, Days since release: {daysSinceRelease}, Hitfactor: {hitFactor}");
                var update = Builders<Song>.Update.Set(x => x.DailyStreams, streams).Inc(y => y.TotalStreams, streams);
                companies.SongCollection.FindOneAndUpdate(x => x.Id == song.Id, update);
            }
            Console.WriteLine($"{DateTime.Now:T} All streams have been successfully updated!");
            try
            {
                Console.WriteLine($"{DateTime.Now:T} Trying to update...");
                companies.SetSongToPublished();
                Console.WriteLine($"{DateTime.Now:T} Updated new songs.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

