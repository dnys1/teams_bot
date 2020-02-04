using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BC.ServerTeamsBot.Data
{
    // A static class for formatting links. Used in this project and the native host.
    public static class LinkFormatter
    {
        public static string BaseUrl = "https://serverlinks.azurewebsites.net/link";

        public static string ImageBaseUrl = "https://serverlinks.azurewebsites.net/images";

        public static string FormatString(string link)
        {
            if (IsDirPath(link))
            {
                var escapedLink = link.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                // Format path as file URI
                if (IsLocalPath(link))
                {
                    link = "file:///" + escapedLink;
                }
                else
                {
                    link = "file:" + escapedLink;
                }

                if (link[link.Length - 1] != Path.AltDirectorySeparatorChar && !Path.HasExtension(link))
                {
                    return link + Path.AltDirectorySeparatorChar;
                }
                else
                {
                    return link;
                }
            } else
            {
                return link;
            }
        }

        // Match all local (i.e. C:/, P:/, etc) and UNC paths
        private static Regex dirRegex = new Regex(@"^(?:[a-zA-Z]:|\\\\[a-z0-9_.$●-]+\\[a-z0-9_.$●-]+)\\(?:[^\\/:*?" + '"' + @"<>|\r\n]+\\)*[^\\/:*?" + '"' + "<>|\r\n]*$");

        // Match only local (i.e. C:/, P:/, etc) paths
        private static Regex localDirRegex = new Regex(@"^[a-zA-Z]:\\(?:[^\\/:*?" + '"' + @"<>|\r\n]+\\)*[^\\/:*?" + '"' + @"<>|\r\n]*$");

        // Matches links of the form "P:/.../..." with the quotes
        private static Regex copyAsPathRegex = new Regex("^\".*\"$");

        // Matches ProjectWise links
        private static Regex projectWiseRegex = new Regex("^pw://");

        // Matches Document URN links
        private static Regex documentURNRegex = new Regex("^url:pw://");

        public static bool IsProperlyFormatted(string link) => dirRegex.IsMatch(link) || projectWiseRegex.IsMatch(link);

        public static bool IsUNCPath(string link) => dirRegex.IsMatch(link) && !localDirRegex.IsMatch(link);

        public static bool IsDirPath(string link) => dirRegex.IsMatch(link);

        public static bool IsLocalPath(string link) => localDirRegex.IsMatch(link);

        public static bool IsCopyAsPathLink(string link) => copyAsPathRegex.IsMatch(link);

        public static bool IsProjectWiseLink(string link) => projectWiseRegex.IsMatch(link);

        public static bool IsDocumentURN(string link) => documentURNRegex.IsMatch(link);
    }
}
