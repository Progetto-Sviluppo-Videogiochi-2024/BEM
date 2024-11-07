using UnityEngine;

[CreateAssetMenu(fileName = "AIAgentConfig", menuName = "AIAgent/Config")]
public class AIAgentConfig : ScriptableObject
{
    public float maxTime = 1.0f; // Tempo massimo tra gli aggiornamenti del path
    public float maxDistance = 1.0f; // Distanza massima per aggiornare il percorso
}
