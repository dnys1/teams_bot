using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BC.ServerTeamsBot.Data
{
    public static class LinkFormatter
{
        public static string BaseUrl = "https://bc-teams-bot.azurewebsites.net/link";

        public static string ImageBaseUrl = "https://bc-teams-bot.azurewebsites.net/images";

        public static string FormatString(string link)
        {
            // Format path as file URI
            if (localDirRegex.IsMatch(link.ToLower()))
            {
                link = "file:///" + link.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
            else
            {
                link = "file:" + link.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }

            if (link[link.Length - 1] != Path.AltDirectorySeparatorChar && !Path.HasExtension(link))
            {
                return link + Path.AltDirectorySeparatorChar;
            } else
            {
                return link;
            }
        }

        // Match all local (i.e. C:/, P:/, etc) and UNC paths
        private static Regex dirRegex = new Regex(@"^(?:[a-z]:|\\\\[a-z0-9_.$●-]+\\[a-z0-9_.$●-]+)\\(?:[^\\/:*?" + '"' + @"<>|\r\n]+\\)*[^\\/:*?" + '"' + "<>|\r\n]*$");

        // Match only local (i.e. C:/, P:/, etc) paths
        private static Regex localDirRegex = new Regex(@"^[a-z]:\\(?:[^\\/:*?" + '"' + @"<>|\r\n]+\\)*[^\\/:*?" + '"' + @"<>|\r\n]*$");

        private static Regex copyAsPathRegex = new Regex("^\".*\"$");

        public static bool IsProperlyFormatted(string link) => dirRegex.IsMatch(link.ToLower());

        public static bool IsCopyAsPathLink(string link) => copyAsPathRegex.IsMatch(link);
}
}
