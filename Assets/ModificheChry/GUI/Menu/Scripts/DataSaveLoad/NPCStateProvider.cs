using UnityEngine;

public class NPCBlendStateProvider : MonoBehaviour, INPCStateProvider
{
    public string GetCurrentState() => "Movement";
}
