namespace CoDFilesCleaner
{
    public sealed class Program
    {
    static void Main()
        {
            Console.Title = "CoDFilesCleaner by Praveshan";
            if (!IsSupportedGameDir())
            {
                DisplayMessage("Please run this appliction in your Advanced Warfare, Ghosts or Modern Warfare Remastered Game Directory.", ConsoleColor.Red);
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
                return;
            }

            // Options
            do
            {
                Console.Clear();
                DisplayMessage("[C] Clean your game files", ConsoleColor.Cyan);
                DisplayMessage("[U] Undo cleaning, match Steam install (MWR only)", ConsoleColor.Cyan);

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.C:
                        Console.WriteLine("Cleaning...");
                        Files.CleanAllFiles();
                        DisplayMessage("File Changes were saved at CoDFileCleaner.log", ConsoleColor.Cyan);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;

                    case ConsoleKey.U:
                        if (!File.Exists("h1_sp64_ship.exe") || !File.Exists("h1_mp64_ship.exe"))
                        {
                            DisplayMessage("This only supports Modern Warfare Remastered", ConsoleColor.Red);
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey(true);
                            break;
                        }
                        Files.UndoClean();
                        DisplayMessage("File Changes were saved at CoDFileCleaner.log", ConsoleColor.Cyan);
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                }

            } while (true);
            
        }
        public static void DisplayMessage(string message, ConsoleColor messageColor)
        {
            ConsoleColor currentColor = Console.ForegroundColor;
            Console.ForegroundColor = messageColor;
            Console.WriteLine(message);
            Console.ForegroundColor = currentColor;
        }
        private static bool IsSupportedGameDir()
        {
            string[] supportedGames = ["s1_mp64_ship.exe", "s1_sp64_ship.exe", "iw6mp64_ship.exe", "iw6sp64_ship.exe", "h1_sp64_ship.exe", "h1_mp64_ship.exe"];
            return supportedGames.Intersect(Directory.EnumerateFiles(@".\", "*.exe").Select(f => Path.GetFileName(f))).Any();
        }
    }
}
