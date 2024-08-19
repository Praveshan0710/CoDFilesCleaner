
namespace CoDFilesCleaner
{
    internal sealed class Files
    {
        readonly static string[] languageDirs =
            ["english",
            "english_safe",
            "french",
            "german",
            "russian",
            "polish",
            "japanese_partial",
            "korean",
            "portuguese",
            "spanish",
            "italian",
            "simplified_chinese",
            "traditional_chinese"];
        public static void UndoClean()
        {
            Console.Clear();
            Console.WriteLine("Undoing clean...");
            ReadOnlySpan<string> dirs = ["zone", "raw"];
            var currentDirName = Path.GetFileName(Directory.GetCurrentDirectory());

            var sw = new StreamWriter("CoDFilesCleaner.log", true);

            if (Directory.Exists(dirs[0]))
            {
                /*ReadOnlySpan<string> languageDirs = 
                    ["english",
                    "english_safe",
                    "french",
                    "german",
                    "russian",
                    "polish",
                    "japanese_partial",
                    "korean",
                    "portuguese",
                    "spanish",
                    "italian",
                    "simplified_chinese",
                    "traditional_chinese"];*/

                foreach (var languageDir in languageDirs)
                {
                    var langDir = Path.Combine("zone", languageDir);
                    if (Directory.Exists(langDir))
                    {
                        Directory.Move(langDir, languageDir);
                        sw.WriteLine($"Moved directory {langDir} to {currentDirName}");
                    }
                }
            }

            foreach (var dir in dirs)
            {
                if (Directory.Exists(dir))
                {
                    foreach (var file in new DirectoryInfo(dir).EnumerateFiles("*", SearchOption.AllDirectories))
                    {
                        if (!File.Exists(file.Name))
                        {
                            file.MoveTo(file.Name);
                            //Debug.WriteLine($"Moved {file} to {currentDirName}");
                            sw.WriteLine($"Moved {file} to {currentDirName}");
                        }
                        else
                        {
                            //byte by byte compare
                            if (FilesAreSame(file.FullName, file.Name))
                            {
                                file.Delete();
                                Program.DisplayMessage($"Duplicate {file.Name} was deleted.", ConsoleColor.DarkYellow);
                                sw.WriteLine($"Duplicate {file.FullName} was deleted.");
                            }
                            else
                            {
                                Program.DisplayMessage($"{file.Name} already exists in {currentDirName} but, does not contain the same content as {file.FullName}, skipping it", ConsoleColor.Red);
                                sw.WriteLine($"{file.Name} already exists in {currentDirName} but, does not contain the same content as {file.FullName}");
                            }
                        }
                    }
                }
            }

            foreach (var dir in dirs)
            {
                if (Directory.Exists(dir))
                {
                    if (!Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories).Any())
                    {
                        Directory.Delete(dir, true);
                        //Debug.WriteLine($"Removed empty directory {dir}");
                        sw.WriteLine($"Removed empty directory {dir}");
                    }
                    else
                    {
                        Program.DisplayMessage($"Couldn't remove {dir}, please review and remove it manually", ConsoleColor.Yellow);
                        sw.WriteLine($"Could not remove {dir}");
                    }
                }
            }
            sw.Dispose();
            Program.DisplayMessage("Logged changes at CoDFilesCleaner.log", ConsoleColor.Cyan);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        /*private static void LogChange(string message)
        {
            using var sw = new StreamWriter("CoDFileCleaner.log", true);
            sw.WriteLine(message);
        }*/

        public static bool FilesAreSame(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
                return true;

            // Open the two files.
            fs1 = File.OpenRead(file1);
            fs2 = File.OpenRead(file2);

            // Check the file sizes. If they are not the same, the files
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                fs1.Dispose();
                fs2.Dispose();
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Dispose();
            fs2.Dispose();

            // Return the success of the comparison. "file1byte" is
            // equal to "file2byte" at this point only if the files are
            // the same.
            return ((file1byte - file2byte) == 0);
        }

        public static void CleanAllFiles()
        {
            Console.Clear();
            Console.WriteLine("Cleaning files...");
            
            var sw = new StreamWriter("CoDFilesCleaner.log", true);
            
            const string zonePath = "zone";
            const string bikPath = @"raw\video";

            /*ReadOnlySpan<string> languageDirs =
            ["english",
            "english_safe",
            "french",
            "german",
            "russian",
            "polish",
            "japanese_partial",
            "korean",
            "portuguese",
            "spanish",
            "italian",
            "simplified_chinese",
            "traditional_chinese"];*/

            foreach (var file in new DirectoryInfo(Directory.GetCurrentDirectory()).EnumerateFiles())
            {
                switch (file.Extension)
                {
                    case ".ff":
                    case ".pak":
                        Directory.CreateDirectory(zonePath);
                        file.MoveTo(Path.Combine(zonePath, file.Name));
                        sw.WriteLine($"Moved {file.Name} to {zonePath}");
                        break;

                    case ".bik":
                        Directory.CreateDirectory(bikPath);
                        file.MoveTo(Path.Combine(bikPath, file.Name));
                        sw.WriteLine($"Moved {file.Name} to {bikPath}");
                        break;
                }
            }
            
            foreach (var dir in languageDirs)
            {
                if (Directory.Exists(dir))
                {
                    string destination = Path.Combine(zonePath, dir);
                    Directory.Move(dir, destination);
                    sw.WriteLine($"Moved {dir} to {destination}");
                }
            }

            sw.Dispose();
            Program.DisplayMessage("Logged changes at CoDFilesCleaner.log", ConsoleColor.Cyan);
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);

        }
    }
}