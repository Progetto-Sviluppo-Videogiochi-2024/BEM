using System;
using UnityEngine;

[Serializable]
public class SerializableGuid : IEquatable<SerializableGuid>
{
    [SerializeField, HideInInspector] public uint Part1;
    [SerializeField, HideInInspector] public uint Part2;
    [SerializeField, HideInInspector] public uint Part3;
    [SerializeField, HideInInspector] public uint Part4;

    // Proprietà per ottenere un GUID vuoto
    public static SerializableGuid Empty => new(0, 0, 0, 0);

    // Costruttore per inizializzare i 4 partizioni del GUID
    public SerializableGuid(uint val1, uint val2, uint val3, uint val4)
    {
        Part1 = val1;
        Part2 = val2;
        Part3 = val3;
        Part4 = val4;
    }

    // Costruttore per creare un SerializableGuid da un Guid standard
    public SerializableGuid(Guid guid)
    {
        byte[] bytes = guid.ToByteArray();
        Part1 = BitConverter.ToUInt32(bytes, 0);
        Part2 = BitConverter.ToUInt32(bytes, 4);
        Part3 = BitConverter.ToUInt32(bytes, 8);
        Part4 = BitConverter.ToUInt32(bytes, 12);
    }

    // Metodo per creare un nuovo GUID
    public static SerializableGuid NewGuid() => Guid.NewGuid().ToSerializableGuid();

    // Metodo per creare un SerializableGuid da una stringa esadecimale
    public static SerializableGuid FromHexString(string hexString)
    {
        if (hexString.Length != 32) return Empty;

        return new SerializableGuid
        (
            Convert.ToUInt32(hexString[..8], 16), // Equivalente a Convert.ToUInt32(hexString.Substring(0, 8), 16)
            Convert.ToUInt32(hexString.Substring(8, 8), 16),
            Convert.ToUInt32(hexString.Substring(16, 8), 16),
            Convert.ToUInt32(hexString.Substring(24, 8), 16)
        );
    }

    // Metodo per convertire il SerializableGuid in una stringa esadecimale
    public string ToHexString() => $"{Part1:X8}{Part2:X8}{Part3:X8}{Part4:X8}";

    // Metodo per convertire il SerializableGuid in un Guid standard
    public Guid ToGuid()
    {
        var bytes = new byte[16];
        BitConverter.GetBytes(Part1).CopyTo(bytes, 0);
        BitConverter.GetBytes(Part2).CopyTo(bytes, 4);
        BitConverter.GetBytes(Part3).CopyTo(bytes, 8);
        BitConverter.GetBytes(Part4).CopyTo(bytes, 12);
        return new Guid(bytes);
    }

    // Conversione implicita da SerializableGuid a Guid
    public static implicit operator Guid(SerializableGuid serializableGuid) => serializableGuid.ToGuid();

    // Conversione implicita da Guid a SerializableGuid
    public static implicit operator SerializableGuid(Guid guid) => new(guid);

    // Implementazione del confronto tra due SerializableGuid
    public override bool Equals(object obj) => obj is SerializableGuid guid && this.Equals(guid);

    public bool Equals(SerializableGuid other) => Part1 == other.Part1 && Part2 == other.Part2 && Part3 == other.Part3 && Part4 == other.Part4;

    // Implementazione dell'hashcode per il SerializableGuid
    public override int GetHashCode() => HashCode.Combine(Part1, Part2, Part3, Part4);

    // Operatore di uguaglianza
    public static bool operator ==(SerializableGuid left, SerializableGuid right) => left.Equals(right);

    // Operatore di disuguaglianza
    public static bool operator !=(SerializableGuid left, SerializableGuid right) => !(left == right);
}

public static class GuidExtensions
{
    // Metodo di estensione per convertire Guid in SerializableGuid
    public static SerializableGuid ToSerializableGuid(this Guid guid) => new(guid);
}
