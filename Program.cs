namespace CoDFilesCleaner
{
    public sealed class Program
    {
    static void Main()
        {
            Console.Title = "CoD Files Cleaner";
            Console.CursorVisible = false;
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
                DisplayMessage("[U] Undo cleaning to match the Steam Install", ConsoleColor.Cyan);

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.C:
                        Files.CleanAllFiles();
                        break;

                    case ConsoleKey.U:
                        Files.UndoClean();
                        break;
                }

            } while (true);
            
        }
        public static void DisplayMessage(string message, ConsoleColor messageColor)
        {
            Console.ForegroundColor = messageColor;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        private static bool IsSupportedGameDir()
        {
            string[] supportedGames = ["s1_mp64_ship.exe", "s1_sp64_ship.exe", "iw6mp64_ship.exe", "iw6sp64_ship.exe", "h1_sp64_ship.exe", "h1_mp64_ship.exe"];
            return supportedGames.Intersect(Directory.EnumerateFiles(@".\", "*.exe").Select(f => Path.GetFileName(f))).Any();
        }
    }
}
