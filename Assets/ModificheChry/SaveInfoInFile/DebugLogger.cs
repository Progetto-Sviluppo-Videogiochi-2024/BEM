using System.IO;
using UnityEngine;

public static class DebugLogger
{
    private static string folderPath;
    private static string logFilePath;

    static DebugLogger()
    {
        // Crea una directory per i log nella cartella della build
        folderPath = Path.Combine(Application.dataPath, "../Logs");
        logFilePath = Path.Combine(folderPath, "debug_log.txt");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        File.WriteAllText(logFilePath, string.Empty);
    }

    public static void Log(string message)
    {
        // Scrive il messaggio nel file di log
        File.AppendAllText(logFilePath, $"{System.DateTime.Now}: {message}\n\n");
    }

    public static void LogWarning(string message)
    {
        Log($"WARNING: {message}");
    }

    public static void LogError(string message)
    {
        Log($"ERROR: {message}");
    }
}
