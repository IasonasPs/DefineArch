using Mono.Cecil;
using System.Collections;
using System.IO;
using System.Reflection.PortableExecutable;

namespace DefineArch
{
    internal class OrganizeByArchitecture
    {

        public static string? GetUserSpecifiedPath(string[] args)
        {
            if (args.Length > 0)
            {
                // The first argument is assumed to be the path
                return args[0];
            }
            else
            {
                return null; // No arguments were provided
            }
        }

        public static bool Is64Architecture(string item)
        {
            using (FileStream fileStream = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (PEReader reader = new(fileStream, PEStreamOptions.LeaveOpen))
                {
                    var machine = reader.PEHeaders.CoffHeader.Machine;

                    bool PeCheck = machine == System.Reflection.PortableExecutable.Machine.Amd64 ||
                    machine == System.Reflection.PortableExecutable.Machine.IA64;

                    fileStream.Position = 0;
                    var currentItem = AssemblyDefinition.ReadAssembly(item);
                    var arch = currentItem.MainModule.Architecture;

                    var cecilCheck = arch == TargetArchitecture.IA64 || arch == TargetArchitecture.AMD64;
                    var peCheck = reader.PEHeaders.CoffHeader.Machine.ToString() == "Amd64";

                    return cecilCheck && peCheck;
                }
            }
        }

        public static void RetryFileOperation(Action fileAction, int maxRetries = 8, int delayMs = 200)
        {
            int attempts = 0;
            while (true)
            {
                try
                {
                    fileAction();
                    break; // Success
                }
                catch (IOException) when (attempts < maxRetries)
                {
                    attempts++;
                    Thread.Sleep(delayMs); // Wait before retrying
                }
            }
        }

        public static void OrganizeDllFilesByArchitecture(string folder, bool moveToArchDir)
        {
            if (Directory.Exists(folder))
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var report = Path.Combine(desktopPath, "report.csv");
                Console.WriteLine(report);

                using (var streamWriter = new StreamWriter(report, false))
                {
                    DirectoryInfo? x86 = null, x64 = null;
                    if (moveToArchDir)
                        CreateArchitectureFolders(folder, out x86, out x64);

                    string[] files = Directory.GetFiles(folder);

                    // Write CSV header
                    streamWriter.WriteLine("FileName,Architecture");

                    try
                    {
                        foreach (var item in files)
                        {
                            if (Path.GetExtension(item).Equals(".dll", StringComparison.OrdinalIgnoreCase))
                            {
                                string arch = Is64Architecture(item) ? "x64" : "x86";

                                if (moveToArchDir)
                                {
                                    RetryFileOperation(() =>
                                    {
                                        var destDir = Is64Architecture(item) ? x64!.FullName : x86!.FullName;
                                        var destPath = Path.Combine(destDir, Path.GetFileName(item));
                                        if (!File.Exists(destPath))
                                            File.Move(item, destPath);
                                    });
                                }

                                streamWriter.WriteLine($"{Path.GetFileName(item)},{arch}");
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        // Optionally log or handle exception
                    }
                }
            }
        }

        private static void CreateArchitectureFolders(string folder, out DirectoryInfo x86, out DirectoryInfo x64)
        {
            x86 = Directory.CreateDirectory(Path.Combine(folder, Architecture.x86.ToString()));
            x64 = Directory.CreateDirectory(Path.Combine(folder, Architecture.x64.ToString()));
        }
    }
}