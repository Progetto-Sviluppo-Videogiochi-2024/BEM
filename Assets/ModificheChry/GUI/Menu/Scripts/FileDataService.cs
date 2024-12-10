using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataService : IDataService
{
    ISerializer serializer;
    string dataPath;
    string fileExtension;

    public FileDataService(ISerializer serializer)
    {
        this.dataPath = Application.persistentDataPath;
        this.fileExtension = "json";
        this.serializer = serializer;
    }

    string GetFilePath(string fileName) => Path.Combine(dataPath, string.Concat(fileName, fileExtension));

    public void Delete(string name)
    {
        string fileLocation = GetFilePath(name);

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
            if (Path.GetExtension(path) == fileExtension) yield return Path.GetFileNameWithoutExtension(path);
        }
    }

    public GameData Load(string name)
    {
        string fileLocation = GetFilePath(name);

        if (!File.Exists(fileLocation)) throw new ArgumentException($"No persisted GameData found with the name '{name}'.");
        return serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
    }

    public void Save(GameData data, bool overwrite = true)
    {
        string fileLocation = GetFilePath(data.name);

        if (!overwrite && File.Exists(fileLocation)) throw new IOException($"The file {data.name}.{fileExtension} already exists and can't be overwritten.");
        File.WriteAllText(fileLocation, serializer.Serialize(data));
    }
}
