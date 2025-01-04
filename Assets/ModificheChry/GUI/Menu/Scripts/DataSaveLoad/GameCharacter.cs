using System;
using UnityEngine;

public class GameCharacter : MonoBehaviour, IBind<PlayerData>
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid(); // ID univoco del giocatore
    [SerializeField] public PlayerData data; // Dati del giocatore

    public void Bind(PlayerData data) // Carica da file
    {
        this.data = data;
        this.data.Id = Id;
        transform.SetPositionAndRotation(data.position, data.rotation);
        if (TryGetComponent(out Player player))
        {
            player.health = data.health;
            player.sanitaMentale = data.sanitaMentale;
            player.isDead = data.isDead;
            player.menteSana = data.menteSana;
            player.transform.GetChild(0).SetPositionAndRotation(data.positionVcam, data.rotationVcam); // Set "CameraFollowPosition"
            DebugLogger.Log($"Load Player: {player.health} {player.sanitaMentale} {player.isDead} {player.menteSana}");
        }
    }

    public void SavePlayerData() // Salva su file
    {
        data.position = transform.position;
        data.rotation = transform.rotation;

        if (TryGetComponent(out Player player))
        {
            data.health = player.health;
            data.sanitaMentale = player.sanitaMentale;
            data.isDead = player.isDead;
            data.menteSana = player.menteSana;
            var vcam = player.transform.GetChild(0); // Get "CameraFollowPosition"
            data.positionVcam = vcam.position;
            data.rotationVcam = vcam.rotation;
            DebugLogger.Log($"Save Player: {player.health} {player.sanitaMentale} {player.isDead} {player.menteSana}");
        }
    }
}

[Serializable]
public class PlayerData : ISaveable
{
    [field: SerializeField] public SerializableGuid Id { get; set; } // ID univoco del giocatore
    public Vector3 position; // Posizione del giocatore
    public Quaternion rotation; // Rotazione del giocatore
    public int health; // Salute attuale
    public int sanitaMentale; // Salute mentale attuale
    public bool isDead; // Stato del giocatore (vivo/morto)
    public bool menteSana; // Stato della salute mentale ("sano"/"malato")
    public Vector3 positionVcam; // Posizione della Virtual Camera
    public Quaternion rotationVcam; // Rotazione della Virtual Camera
}
