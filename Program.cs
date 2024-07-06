namespace CoDFilesCleaner
{
    internal class Program
    {
    static void Main()
        {
/*            ReadOnlySpan<string> supportedGames = ["s1_mp64_ship.exe", "s1_sp64_ship.exe"];
            Console.Title = "CoD Files Cleaner";
            foreach (var s in supportedGames)
            {
                Console.WriteLine(s);
                if (File.Exists(s))
                {
                    break;
                }
                DisplayMessage("Please run this appliction in your Advanced Warfare Game Directory.", ConsoleColor.Red);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                return;
            }*/
            if (!File.Exists("s1_mp64_ship.exe") && !File.Exists("s1_sp64_ship.exe"))
            {
                DisplayMessage("Please run this appliction in your Advanced Warfare Game Directory.", ConsoleColor.Red);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                return;
            }
            CleanFiles();
            DisplayMessage("File Changes were saved at ./CoDFileCleaner.log", ConsoleColor.Cyan);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
        private static void DisplayMessage(string message, ConsoleColor messageColor)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = messageColor;
            Console.WriteLine(message);
            Console.ForegroundColor = currentColor;
        }
        private static void CleanFiles()
        {
            DisplayMessage("Cleaning Files...\n", ConsoleColor.Blue);
            CleanFastFiles();
            CleanPakFiles();
            CleanBikFiles();
            DisplayMessage("File clean up was completed!\n", ConsoleColor.Green);
        }
        private static void CleanFastFiles()
        {
            DisplayMessage("Cleaning .ff files...\n", ConsoleColor.Blue);
            List<string> engFastFiles = [];
            List<string> commonFastFiles = [];

            foreach (var fastFile in Directory.EnumerateFiles("./", "*.ff"))
            {
                if (fastFile.StartsWith("./eng_"))
                {
                    engFastFiles.Add(fastFile);
                }
                else
                {
                    commonFastFiles.Add(fastFile);
                }
            }
            if (engFastFiles.Count > 0)
            {
                const string engZoneDir = @"zone\english";
                if (!Directory.Exists(engZoneDir))
                {
                    Directory.CreateDirectory(engZoneDir);
                    DisplayMessage($"\nCreated {engZoneDir}\n", ConsoleColor.Magenta);
                    LogChange($"\nCreated {engZoneDir}\n");
                }
                foreach (var f in engFastFiles)
                {
                    File.Move(f, Path.Combine(engZoneDir, Path.GetFileName(f)));
                    Console.WriteLine($"{f} was moved to {engZoneDir}");
                    LogChange($"{f} was moved to {engZoneDir}");
                }
            }
            if (commonFastFiles.Count > 0)
            {
                const string zoneDir = @"zone";
                if (!Directory.Exists(zoneDir))
                {
                    Directory.CreateDirectory(zoneDir);
                    DisplayMessage($"\nCreated {zoneDir}\n", ConsoleColor.Magenta);
                    LogChange($"\nCreated {zoneDir}\n");
                }
                foreach (var f in commonFastFiles)
                {
                    File.Move(f, Path.Combine(zoneDir, Path.GetFileName(f)));
                    Console.WriteLine($"{f} was moved to {zoneDir}");
                    LogChange($"{f} was moved to {zoneDir}");
                }
            }
            DisplayMessage("\nCompleted .ff file clean...\n", ConsoleColor.Green);
            //LogChange("Completed .ff file clean...\n");
        }
        private static void CleanPakFiles()
        {
            DisplayMessage("\nCleaning .pak files...\n", ConsoleColor.Blue);
            List<string> pakFiles = [];
            foreach (var pakfile in Directory.GetFiles("./", "*.pak"))
            {
                pakFiles.Add(pakfile);
            }
            if (pakFiles.Count > 0)
            {
                const string zoneDir = @"zone";
                if (!Directory.Exists(zoneDir))
                {
                    Directory.CreateDirectory(zoneDir);
                    DisplayMessage($"\n{zoneDir} was created\n", ConsoleColor.Magenta);
                    LogChange($"\n{zoneDir} was created\n");
                }
                
                foreach (var f in pakFiles)
                {
                    File.Move(f, Path.Combine(zoneDir, Path.GetFileName(f)));
                    Console.WriteLine($"{f} was moved to {zoneDir}");
                    LogChange($"{f} was moved to {zoneDir}");
                }
            }
            DisplayMessage("\nCompleted .pak file clean...\n", ConsoleColor.Green);
            //LogChange("\nCompleted .pak file clean...\n");
        }
        private static void CleanBikFiles()
        {
            DisplayMessage("\nCleaning .bik files...\n", ConsoleColor.Blue);
            List<string> bikFiles = [];
            foreach (var pakfile in Directory.GetFiles("./", "*.bik"))
            {
                bikFiles.Add(pakfile);
            }
            if (bikFiles.Count > 0)
            {
                const string videoDir = @"raw\video";
                if (!Directory.Exists(videoDir))
                {
                    Directory.CreateDirectory(videoDir);
                    DisplayMessage($"\nCreated {videoDir}\n", ConsoleColor.Magenta);
                    LogChange($"\nCreated {videoDir}\n");
                }

                foreach (var f in bikFiles)
                {
                    File.Move(f, Path.Combine(videoDir, Path.GetFileName(f)));
                    Console.WriteLine($"{f} was moved to {videoDir}");
                    LogChange($"{f} was moved to {videoDir}");
                }
            }
            DisplayMessage("\nCompleted .bik file clean...\n", ConsoleColor.Green);
            //LogChange("\nCompleted .bik file clean...\n");
        }
        private static void LogChange(string message)
        {
            using var sw = new StreamWriter("./CoDFileCleaner.log", true);
            sw.WriteLine(message);
        }
    }
}
/*private static void CheckForGames()
{
    string[] supportedGames = ["s1_mp64_ship.exe", "s1_sp64_ship.exe"];
}*/
