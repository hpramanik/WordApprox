namespace WordApprox_Core.Utilities
{
    public class About
    {
        public const string Author = "Himangshu SHekhar Pramanik";
        public const string Email = "hpramanik@outlook.in";
        public const string Version = "v0.0.1";
        public const string ProductName = "WordApprox FAQ [Core System]";    

        public override string ToString()
        {
            string output = string.Empty;
            output += $"{ProductName}\n";
            output += $"Version: {Version}\n";
            output += $"Author: {Author}\n";
            output += $"Email: {Email}\n";
            return output;
        }     
    }
}