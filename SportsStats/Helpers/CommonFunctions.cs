namespace SportsStats.Helpers
{
    public static class CommonFunctions
    {
        public static string TrimPlayerName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            if (name.Length > 14)
            {
                return name.Substring(0, 14) + "...";
            }
            else
            {
                return name;
            }
        }

        public static string TrimLeagueName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return string.Empty;

            if (name.Length > 35)
            {
                return name.Substring(0, 35) + "...";
            }
            else
            {
                return name;
            }
        }
    }
}
