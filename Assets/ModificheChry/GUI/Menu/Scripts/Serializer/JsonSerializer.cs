using UnityEngine;

public class JsonSerializer : ISerializer
{
    public string Serialize<T>(T obj)
    {
        // Assicurati che venga chiamato OnBeforeSerialize prima di serializzare
        if (obj is ISerializationCallbackReceiver receiver)
        {
            receiver.OnBeforeSerialize();
        }

        return JsonUtility.ToJson(obj, true);
    }

    public T Deserialize<T>(string json)
    {
        T obj = JsonUtility.FromJson<T>(json);

        // Assicurati che venga chiamato OnAfterDeserialize dopo deserializzazione
        if (obj is ISerializationCallbackReceiver receiver)
        {
            receiver.OnAfterDeserialize();
        }

        return obj;
    }
}
