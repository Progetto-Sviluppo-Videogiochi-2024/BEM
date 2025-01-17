
// Interfaccia per differenziare gli attacchi dei mutanti
using System.Collections.Generic;
using UnityEngine;

public interface IAttackAI
{
    [Tooltip("Riferimento all'agente AI")] AIAgent Agent { get; set; }
    [Tooltip("Dizionario: nomeAttacco => (float chance, int damage)")] Dictionary<string, (float probability, int damage)> Attacks { get; }
    [Tooltip("Riferimento allo stato attacco dell'AI")] AIAttackState AttackState { get; set; }
    [Tooltip("Attacco corrente dell'AI")] string CurrentAttack { get; set; }
    [Tooltip("Danno dell'attacco corrente dell'AI")] int CurrentDamage { get; set; }
    [Tooltip("Indica se il flyKick è stato eseguito (solo per Scarnix)")] bool hasPerformedFlyKick { get; set; }
    void PerformRandomAttack(AIAgent agent);
}
