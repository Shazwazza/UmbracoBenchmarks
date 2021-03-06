﻿using System;
using System.IO;

namespace UmbracoBenchmarks.Tools
{
    public class ConsoleHelper
    {
        public static void Setup(ConsoleArgs consoleArgs)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Booting configured umbraco version {consoleArgs.UmbracoVersion}...");
        }

        public static ConsoleArgs ParseArgs(string[] args)
        {
            if (args.Length != 4)
                throw new InvalidOperationException("args length must be 3");

            if (!Directory.Exists(args[0])) throw new InvalidOperationException($"The folder {args[0]} doesn't exist");
            if (!Guid.TryParse(args[2], out var guidRunId)) throw new InvalidOperationException($"The {args[2]} isn't a guid");
            if (!int.TryParse(args[3], out var index)) throw new InvalidOperationException($"The {args[3]} isn't an INT");
            return new ConsoleArgs(new DirectoryInfo(args[0]), args[1], guidRunId, index);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
