using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataService : IDataService
{
    ISerializer serializer; // Serializzatore per la conversione dei dati in stringa e viceversa
    string dataPath; // Percorso della cartella di salvataggio
    string fileExtension; // Estensione dei file di salvataggio (es. "json")

    public FileDataService(ISerializer serializer)
    {
        dataPath = Application.persistentDataPath + "/Saves";
        fileExtension = "json";
        this.serializer = serializer;

        // Crea la directory se non esiste
        if (!Directory.Exists(dataPath))
        {
            Directory.CreateDirectory(dataPath);
            Debug.Log($"Directory di salvataggio creata: {dataPath}");
        }
    }


    public bool DoesSaveExist(string slotFileName) => File.Exists(GetFilePath(slotFileName));

    string GetFilePath(string fileName) => Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));

    public bool SearchSlotFileByUI(string savedSlot, string UISlot) => savedSlot[^1] == UISlot[^1];

    public void Delete(string fileName)
    {
        string fileLocation = GetFilePath(fileName);

        if (File.Exists(fileLocation)) File.Delete(fileLocation);
    }

    public void DeleteAll()
    {
        foreach (string filePath in Directory.GetFiles(dataPath)) File.Delete(filePath);
    }

    public List<string> ListSaves() => Directory.Exists(dataPath) 
        ? new(Array.ConvertAll(Directory.GetFiles(dataPath, "Slot *.json"), Path.GetFileNameWithoutExtension)) 
        : throw new DirectoryNotFoundException($"La directory specificata non esiste: {dataPath}");

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
