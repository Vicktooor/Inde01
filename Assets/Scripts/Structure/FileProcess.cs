using System.Collections.Generic;
using System.IO;

public class FileProcess
{
    public static List<string> AUTORIZED_SUFIX = new List<string>() { "prefab", "asset", "png", "json", "wav", "mp3", "dat", "gd" };

    public static string[] GetFileNames(string targetDirectory, bool includeSufix = false, bool recursive = false)
    {
        List<string> filesPath = new List<string>();
        ProcessDirectory(targetDirectory, filesPath, includeSufix, recursive);
        return filesPath.ToArray();
    }

    public static string[] GetFileNames(string targetDirectory, List<string> autorizedSuffix, bool includeSuffix = false, bool recursive = false)
    {
        List<string> filesPath = new List<string>();
        ProcessDirectory(targetDirectory, filesPath, true, recursive);
        List<string> files = filesPath.FindAll(f => autorizedSuffix.Contains(f.Split(new char[1] { '.' })[1]));
        int count = files.Count;
        for (int i = 0; i < count; i++) files[i] = files[i].Split(new char[1] { '.' })[0];
        return files.ToArray();
    }

    private static void ProcessDirectory(string targetDirectory, List<string> pathArray, bool includeSufix = false, bool recursive = false)
    {
        string[] fileEntries = Directory.GetFiles(targetDirectory);
        foreach (string fileName in fileEntries)
            ProcessFiles(fileName, pathArray, includeSufix);

        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        foreach (string subdirectory in subdirectoryEntries)
        {
            if (recursive) ProcessDirectory(subdirectory, pathArray, includeSufix, recursive);
        }
    }

    private static void ProcessFiles(string path, List<string> pathArray, bool includeSufix = false)
    {
        string[] cutPath = path.Split(new char[3] { '/', '.', '\\' });
        string sufix = cutPath[cutPath.Length - 1];
        if (sufix == "meta") return;
        else if (AUTORIZED_SUFIX.Contains(sufix))
        {
            if (!includeSufix)
            {
                string test = cutPath[cutPath.Length - 2];
                if (!pathArray.Contains(test)) pathArray.Add(test);
            }
            else pathArray.Add(cutPath[cutPath.Length - 2] + "." + sufix);
        }
    }

    public static string GetFileURI(string path)
    {
        return (new System.Uri(path)).AbsoluteUri;
    }
}
