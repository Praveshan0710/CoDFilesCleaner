
namespace CoDFilesCleaner
{
    internal class Files
    {
        public static void UndoClean()
        {
            ReadOnlySpan<string> dirs = ["zone", "raw"];
            var currentDirName = Path.GetFileName(Directory.GetCurrentDirectory());

            var sw = new StreamWriter("CoDFilesCleaner.log", true);

            foreach (var dir in dirs)
            {
                if (Directory.Exists(dir))
                {
                    foreach (var file in new DirectoryInfo(dir).EnumerateFiles("*", SearchOption.AllDirectories))
                    {
                        //might need to check if it already exists in the current dir, if it does, check if it contains the exact data,
                        //if it does, delete the one we are holding instead, if not warn the user
                        if (!File.Exists(file.Name))
                        {
                            file.MoveTo(file.Name);
                            //Console.WriteLine($"Moved {file} to {currentDirName}");
                            sw.WriteLine($"Moved {file} to {currentDirName}");
                        }
                        else
                        {
                            //byte by byte compare
                            if (FilesAreSame(file.FullName, file.Name))
                            {
                                file.Delete();
                                Program.DisplayMessage($"Duplicate {file.FullName} was deleted.", ConsoleColor.DarkYellow);
                                sw.WriteLine($"Duplicate {file.FullName} was deleted.");
                            }
                            else
                            {
                                Program.DisplayMessage($"{file.Name} already exists in {currentDirName} but, does not contain the same content as {file.FullName}, skipping it", ConsoleColor.Red);
                                sw.WriteLine($"{file.Name} already exists in {currentDirName} but, does not contain the same content as {file.FullName}");
                            }
                        }
                        if (file.Name.StartsWith("eng"))
                        {
                            Directory.CreateDirectory(@"english");
                            if (!File.Exists(Path.Combine("english", file.Name)))
                            {
                                file.MoveTo(Path.Combine("english", file.Name));
                                sw.WriteLine($"Moved {file.Name} to english"); // bruh
                            }
                            else
                            {
                                if (FilesAreSame(file.FullName, Path.Combine("english", file.Name)))
                                {
                                    file.Delete();
                                    Program.DisplayMessage($"Duplicate {file.FullName} was deleted.", ConsoleColor.DarkYellow);
                                    sw.WriteLine($"Duplicate {file.FullName} was deleted.");
                                }
                                else
                                {
                                    Program.DisplayMessage($"{file.Name} already exists in english but, does not contain the same content as {file.FullName}, skipping it", ConsoleColor.Red);
                                    sw.WriteLine($"{file.Name} already exists in english but, does not contain the same content as {file.FullName}");
                                }
                            }
                        }
                    }
                }
            }

            // remove now empty dirs (they should be empty)
            foreach (var dir in dirs)
            {
                if (Directory.Exists(dir))
                {
                    if (!Directory.EnumerateFiles(dir).Any())
                    {
                        Directory.Delete(dir, true);
                        //Console.WriteLine($"Removed empty directory {dir}");
                        sw.WriteLine($"Removed empty directory {dir}");
                    }
                    else
                    {
                        Program.DisplayMessage($"Could remove {dir}, please review and remove it manually", ConsoleColor.Yellow);
                        sw.WriteLine($"Could not remove {dir}");
                    }
                }
            }

            sw.Dispose();
        }


        private static void LogChange(string message)
        {
            using var sw = new StreamWriter("CoDFileCleaner.log", true);
            sw.WriteLine(message);
        }

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
            string currentDirectoryName = Path.GetFileName(Directory.GetCurrentDirectory());
            FileInfo[]? files = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*", SearchOption.AllDirectories);
            
            var tempList = new List<FileInfo>();
            var sw = new StreamWriter("CoDFilesCleaner.log", true);


            foreach (var file in files.Where(f => f.Name.EndsWith(".ff") || f.Name.EndsWith(".pak")))
            {
                tempList.Add(file);
            }

            var fastFiles = tempList.ToArray();
            
            if (fastFiles.Length > 0)
            {
                const string zonePath = @"zone";
                Directory.CreateDirectory(zonePath);

                foreach(var file in fastFiles)
                {
                    file.MoveTo(Path.Combine(zonePath, file.Name));
                    sw.WriteLine($"Moved {file.Name} to {currentDirectoryName}");
                }
            }

            tempList.Clear();

            foreach (var file in files.Where(f => f.Name.EndsWith(".bik")))
            {
                tempList.Add(file);
            }

            var bikFiles = tempList.ToArray();
            if (bikFiles.Length > 0)
            {
                const string bikPath = @"raw\video";
                Directory.CreateDirectory(bikPath);

                foreach (var file in bikFiles)
                {
                    file.MoveTo(Path.Combine(bikPath, file.Name));
                    sw.WriteLine($"Moved {file.Name} to {currentDirectoryName}");
                }
            }

            tempList.Clear();

            foreach (FileInfo file in files.Where(f => f.Name.StartsWith("eng_")))
            {
                tempList.Add(file);
            }

            var englishFiles = tempList.ToArray();

            if (englishFiles.Length > 0)
            {
                const string englishZonePath = @"zone\english";
                Directory.CreateDirectory(englishZonePath);

                foreach (var file in englishFiles)
                {
                    file.MoveTo(Path.Combine(englishZonePath, file.Name));
                    sw.WriteLine($"Moved {file.Name} to {currentDirectoryName}");
                }
            }

            foreach (var dir in new DirectoryInfo(@".\").EnumerateDirectories("*", SearchOption.AllDirectories))
            {
                if (!dir.EnumerateFiles("*", SearchOption.AllDirectories).Any())
                {
                    dir.Delete();
                    sw.WriteLine($"Removed empty {dir.Name}");
                }
            }

            sw.Dispose();
        }
    }
}

// old mothods
/*        public static void CleanFastFiles()
        {
            string[] engFastFiles = Directory.GetFiles(@".\", @".\eng_*.ff");

            if (engFastFiles.Length > 0)
            {
                Program.DisplayMessage("Cleaning .ff files...\n", ConsoleColor.Cyan);
                const string engZoneDir = @"zone/english";
                if (!Directory.Exists(engZoneDir))
                {
                    Directory.CreateDirectory(engZoneDir);
                    Program.DisplayMessage($"\nCreated {engZoneDir}\n", ConsoleColor.Magenta);
                    LogChange($"\nCreated {engZoneDir}\n");
                }
                foreach (var f in engFastFiles)
                {
                    File.Move(f, Path.Combine(engZoneDir, f));
                    Console.WriteLine($"{f} was moved to {engZoneDir}");
                    LogChange($"{f} was moved to {engZoneDir}");
                }
            }

            string[] commonFastFiles = Directory.GetFiles(@".\", "*.ff");

            if (commonFastFiles.Length > 0)
            {
                const string zoneDir = @"zone";
                if (!Directory.Exists(zoneDir))
                {
                    Directory.CreateDirectory(zoneDir);
                    Program.DisplayMessage($"\nCreated {zoneDir}\n", ConsoleColor.Magenta);
                    LogChange($"\nCreated {zoneDir}\n");
                }
                foreach (var f in commonFastFiles)
                {
                    File.Move(f, Path.Combine(zoneDir, f));
                    Console.WriteLine($"{f} was moved to {zoneDir}");
                    LogChange($"{f} was moved to {zoneDir}");
                }
            }
            Program.DisplayMessage("\nCompleted .ff file clean.\n", ConsoleColor.Green);
        }
        public static void CleanPakFiles()
        {
            string[] pakFiles = Directory.GetFiles("./", "*.pak");
            if (pakFiles.Length > 0)
            {
                Program.DisplayMessage("\nCleaning .pak files...\n", ConsoleColor.Cyan);
                const string zoneDir = @"zone";
                if (!Directory.Exists(zoneDir))
                {
                    Directory.CreateDirectory(zoneDir);
                    Program.DisplayMessage($"\n{zoneDir} was created\n", ConsoleColor.Magenta);
                    LogChange($"\n{zoneDir} was created\n");
                }

                foreach (var f in pakFiles)
                {
                    File.Move(f, Path.Combine(zoneDir, f));
                    Console.WriteLine($"{f} was moved to {zoneDir}");
                    LogChange($"{f} was moved to {zoneDir}");
                }
            }
            Program.DisplayMessage("\nCompleted .pak file clean...\n", ConsoleColor.Green);
        }
        public static void CleanBikFiles()
        {

            string[] bikFiles = Directory.GetFiles("./", "*.bik");
            if (bikFiles.Length > 0)
            {
                Program.DisplayMessage("\nCleaning .bik files...\n", ConsoleColor.Cyan);
                const string videoDir = @"raw\video";
                if (!Directory.Exists(videoDir))
                {
                    Directory.CreateDirectory(videoDir);
                    Program.DisplayMessage($"\nCreated {videoDir}\n", ConsoleColor.Magenta);
                    LogChange($"\nCreated {videoDir}\n");
                }

                foreach (var f in bikFiles)
                {
                    File.Move(f, Path.Combine(videoDir, f));
                    Console.WriteLine($"{f} was moved to {videoDir}");
                    LogChange($"{f} was moved to {videoDir}");
                }
            }
            Program.DisplayMessage("\nCompleted .bik file clean...\n", ConsoleColor.Green);
        }*/


/*foreach (var dir in dirs)
{
    foreach (string subdir in Directory.EnumerateDirectories(dir, "*", SearchOption.AllDirectories)) // Delete the subdirs if they are empty (they really should be)
    {
        try
        {
            //Directory.Delete(subdir);
            throw new IOException("Random error!");
        }
        catch (Exception ex)
        {
            Program.DisplayMessage($"Couldn't delete directory {subdir}\t {ex.Message}", ConsoleColor.Red);
            LogChange($"Couldn't delete directory {subdir}\t {ex.Message}");
        }
    }

    // should be empty and safe to delete now
    try
    {
        //Directory.Delete(dir);
        throw new IOException("Bruh plz");
        if (!Directory.Exists(dir))
        {
            Console.WriteLine($"Removed empty directory {dir}");
            LogChange($"Removed empty directory {dir}");
        }
    }
    catch (Exception ex)
    {
        Program.DisplayMessage($"Couldn't delete directory {dir}\t {ex.Message}", ConsoleColor.Red);
        LogChange($"Couldn't delete directory {dir}\t {ex.Message}");
    }
}*/
