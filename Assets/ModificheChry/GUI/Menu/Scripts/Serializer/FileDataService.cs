using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class FileDataService : IDataService
{
    ISerializer serializer; // Serializzatore per la conversione dei dati in stringa e viceversa
    string dataPath; // Percorso della cartella di salvataggio
    string fileExtension; // Estensione dei file di salvataggio (es. "json")

    public FileDataService(ISerializer serializer)
    {
        dataPath = Application.persistentDataPath;
        fileExtension = "json";
        this.serializer = serializer;
    }

    string GetFilePath(string fileName) => Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));

    public bool SearchSlotFileByUI(string str1, string str2)
    {
        // Estrai i numeri dalle due stringhe
        var numbers1 = Regex.Matches(str1, @"\d+").Cast<Match>().Select(m => m.Value);
        var numbers2 = Regex.Matches(str2, @"\d+").Cast<Match>().Select(m => m.Value);

        return numbers1.Intersect(numbers2).Any(); // Controlla se i numeri estratti sono uguali
    }

    public void Delete(string fileName)
    {
        string fileLocation = GetFilePath(fileName);

        if (!File.Exists(fileLocation)) File.Delete(fileLocation);
    }

    public void DeleteAll()
    {
        foreach (string filePath in Directory.GetFiles(dataPath)) File.Delete(filePath);
    }

    public IEnumerable<string> ListSaves()
    {
        foreach (string path in Directory.EnumerateFiles(dataPath))
        {
            if (!Path.GetFileNameWithoutExtension(path).StartsWith("Slot")) continue; // Non è un file di salvataggio
            if (Path.GetExtension(path) == fileExtension) yield return Path.GetFileNameWithoutExtension(path);
        }
    }

    public GameData Load(string fileName)
    {
        string fileLocation = GetFilePath(fileName);

        if (!File.Exists(fileLocation)) throw new ArgumentException($"No persisted GameData found with the name '{fileName}'.");
        return serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
    }

    public void Save(GameData data, bool overwrite = true)
    {
        string fileLocation = GetFilePath(data.fileName);

        if (!overwrite && File.Exists(fileLocation)) throw new IOException($"The file {data.fileName}.{fileExtension} already exists and can't be overwritten.");
        File.WriteAllText(fileLocation, serializer.Serialize(data));
    }
}
