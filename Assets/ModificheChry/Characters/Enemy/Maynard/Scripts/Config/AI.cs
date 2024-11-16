using UnityEngine;

[CreateAssetMenu(fileName = "AI", menuName = "AIAgent/Config")]
public class AI : ScriptableObject
{ 
    [Header("Detection")]
    #region Settings
    [Tooltip("Tempo massimo tra gli aggiornamenti del path")] public float maxTime; // Tempo massimo tra gli aggiornamenti del path
    [Tooltip("Distanza massima per aggiornare il percorso")] public float maxDistance; // Distanza massima per aggiornare il percorso
    [Tooltip("Distanza massima di vista")] public float maxSightDistance; // Distanza massima di vista (anche di inseguimento)
    [Tooltip("Campo visivo dell'IA")] public float fov; // Campo visivo
    #endregion

    [Header("Movement")]
    #region Movement
    [Tooltip("Distanza da cui l'IA si ferma")] public float stopDistance; // Distanza di stop (anche dal target)
    [Tooltip("Velocità minima dell'IA")] public float minSpeed; // Velocità minima
    [Tooltip("Velocità di camminata dell'IA")] public float walkSpeed; // Velocità di camminata
    [Tooltip("Velocità di corsa dell'IA")] public float runSpeed; // Velocità di corsa
    [Tooltip("Velocità di rotazione dell'IA")] public float rotationSpeed; // Velocità di rotazione
    #endregion

    [Header("Attack")]
    #region Attack
    [Tooltip("Tempo di recupero tra un attacco e l'altro")] public float attackCooldown; // Cooldown tra un attacco e l'altro
    [Tooltip("Distanza massima di attacco in mischia")] public float distanceAttackMelee; // Distanza massima di attacco in mischia
    [Tooltip("Danno dell'attacco da mischia1")] public int melee1Damage; // Danno dell'attacco da mischia1
    [Tooltip("Danno dell'attacco da mischia2")] public int melee2Damage; // Danno dell'attacco da mischia2
    #endregion
}
