namespace LineUpNode.Helpers
{
    public static class IluzjonDateParser
    {
        private static readonly Dictionary<string, int> MonthMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "stycznia", 1 },
            { "lutego", 2 },
            { "marca", 3 },
            { "kwietnia", 4 },
            { "maja", 5 },
            { "czerwca", 6 },
            { "lipca", 7 },
            { "sierpnia", 8 },
            { "września", 9 },
            { "października", 10 },
            { "listopada", 11 },
            { "grudnia", 12 }
        };

        public static string FormatDateTime(string dateText, string timeText)
        {

            var parts = dateText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                throw new ArgumentException($"Niepoprawny format daty: '{dateText}'");

            if (!int.TryParse(parts[0], out int day))
                throw new ArgumentException($"Niepoprawny dzień w dacie: '{dateText}'");

            var monthName = parts[1].ToLower();
            if (!MonthMap.TryGetValue(monthName, out int month))
                throw new ArgumentException($"Niepoprawny miesiąc w dacie: '{dateText}'");
            
            var currentYear = DateTime.Now.Year;
            var candidateDate = new DateTime(currentYear, month, day);
            
            if (candidateDate < DateTime.Now.Date)
            {
                candidateDate = candidateDate.AddYears(1);
            }
            
            var timeParts = timeText.Split(':');
            if (timeParts.Length < 2)
                throw new ArgumentException($"Niepoprawny format godziny: '{timeText}'");

            if (!int.TryParse(timeParts[0], out int hour) || !int.TryParse(timeParts[1], out int minute))
                throw new ArgumentException($"Niepoprawny czas: '{timeText}'");
            
            var fullDateTime = new DateTime(candidateDate.Year, candidateDate.Month, candidateDate.Day, hour, minute, 0);
            return fullDateTime.ToString("yyyy-MM-dd HH:mm");
        }
    }
}