using UnityEngine;

public class WolfStateProvider : MonoBehaviour, INPCStateProvider
{
    public string GetCurrentState() => BooleanAccessor.istance.GetBoolFromThis("wolfDone") ? "Breath" : "Movement";
}
