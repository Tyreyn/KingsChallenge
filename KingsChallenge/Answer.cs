namespace KingsChallenge
{
    using Newtonsoft.Json;

    /// <summary>
    /// King class.
    /// </summary>
    public class King
    {
        /// <summary>
        /// King's ID.
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// King's name.
        /// </summary>
        public string nm { get; set; }

        /// <summary>
        /// King's city.
        /// </summary>
        public string cty { get; set; }

        /// <summary>
        /// King's house.
        /// </summary>
        public string hse { get; set; }

        /// <summary>
        /// King's years of rule.
        /// </summary>
        public string yrs { get; set; }
    }

    /// <summary>
    /// Challenge answer class.
    /// </summary>
    public class Answer
    {
        /// <summary>
        /// URL string to data.
        /// </summary>
        private const string Url = "https://gist.githubusercontent.com/christianpanton" +
            "/10d65ccef9f29de3acd49d97ed423736/raw/b09563bc0c4b318132c7a738e679d4f984ef0048/kings";

        /// <summary>
        /// Main loop.
        /// </summary>
        public static void Main()
        {
            IList<King>? kingsList = GetAndParseData();
            Console.WriteLine($"1. There is {kingsList.Count} monarchs in the list");

            King longestRulingMonarch = kingsList.OrderByDescending(king => GetReignDuration(king)).First();
            Console.WriteLine($"2. Monarch ''{longestRulingMonarch.nm}'' reigned the longest" +
                $" for {GetReignDuration(longestRulingMonarch).TotalDays} days");

            var longestRulingHouse = kingsList.GroupBy(king => king.hse)
                .Select(house => new { House = house.Key, Duration = house.Sum(king => GetReignDuration(king).TotalDays) })
                .OrderByDescending(house => house.Duration)
                .First();

            Console.WriteLine($"3. House ''{longestRulingHouse.House}'' reigned the longest" +
                $" for {longestRulingHouse.Duration} days");

            var commonFirstName = kingsList.GroupBy(king => king.nm.Split(' ')[0]).OrderByDescending(name => name.Count()).First().Key;

            Console.WriteLine($"4. Most common first name was {commonFirstName}");
        }

        /// <summary>
        /// Get data and parse to list of kings class.
        /// </summary>
        /// <returns>
        /// Kings list.
        /// </returns>
        private static IList<King> GetAndParseData()
        {
            string jsonString = new HttpClient().GetStringAsync(Url).Result;
            return JsonConvert.DeserializeObject<IList<King>>(jsonString);
        }

        /// <summary>
        /// Split, analyze reign years and calculate duration.
        /// </summary>
        /// <param name="king">
        /// The king's reign is calculated.
        /// </param>
        /// <returns>
        /// Calculated reign.
        /// </returns>
        private static TimeSpan GetReignDuration(King king)
        {
            List<int> years = new List<int>();
            foreach (var dateString in king.yrs.Split('-'))
            {
                int date;
                if (int.TryParse(dateString, out date))
                {
                    years.Add(date);
                }
                else
                {
                    years.Add(DateTime.Now.Year);
                }
            }

            if (years.Count == 1)
            {
                return new TimeSpan(0);
            }

            DateTime start = new DateTime(years[0], 1, 1);
            DateTime end = new DateTime(years[1], 1, 1);
            return end - start;
        }
    }
}
