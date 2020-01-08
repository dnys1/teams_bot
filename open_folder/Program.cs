using System;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

using BC.ServerTeamsBot.Data;

namespace BC.ServerTeamsBot.OpenFolder
{
    class Message
    {
        public string Path { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string stdin = null;
            int messageLength = -1;
            if (Console.IsInputRedirected)
            {
                using (Stream stream = Console.OpenStandardInput())
                using (StreamWriter w = File.AppendText("log.txt"))
                {
                    // 4 bytes at beginning of each message carrying the
                    // length of the following message
                    byte[] lengthBuffer = new byte[4];
                    stream.Read(lengthBuffer, 0, 4);
                    messageLength = BitConverter.ToInt32(lengthBuffer);

                    if (messageLength > 0)
                    {
                        byte[] messageBuffer = new byte[messageLength];
                        Log($"Message Length: {messageLength}", w);
                        stream.Read(messageBuffer, 0, messageLength);
                        stdin = Console.InputEncoding.GetString(messageBuffer);
                        Log($"Message Received: {stdin}", w);

                        // Parse the message
                        var message = JsonConvert.DeserializeObject<Message>(stdin);

                        if (!message.Path.StartsWith("file://"))
                        {
                            Log($"Not a file:// path: {message.Path}", w);
                        }

                        string path = message.Path.Substring("file:".Length);

                        // Local paths come in as file:///P:/.../
                        // Remove all slashes and add back for UNC path
                        while (path[0] == Path.AltDirectorySeparatorChar)
                        {
                            path = path.Substring(1);
                        }

                        // Make //bcphxfp01/.../ --> \\bcphxfp01\...\
                        path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                        if (!LinkFormatter.IsLocalPath(path))
                        {
                            path = @"\\" + path;
                        }

                        // Must run as explorer.exe "\\UNC\path\" <-- quotes necessary
                        // Two backslashes (\\) refers to My Documents otherwise.
                        path = '"' + path + '"';

                        Log($"Opening folder: {path}", w);

                        // Open the folder
                        try
                        {
                            Process.Start("explorer.exe", path);
                        }
                        catch (Exception e)
                        {
                            Log($"Error occured: {e.ToString()}", w);
                        }
                    }
                }
            }
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            w.WriteLine("  :");
            w.WriteLine($"  :{logMessage}");
            w.WriteLine("-------------------------------");
        }
    }
}
