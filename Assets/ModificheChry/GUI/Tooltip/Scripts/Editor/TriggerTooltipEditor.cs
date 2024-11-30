using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TriggerTooltip))]
public class TriggerTooltipEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Mostra il commento come tooltip
        EditorGUILayout.HelpBox("Da usare per esprimere riflessioni di Stefano su ciò che gli circonda", MessageType.Info);

        // Ottieni il riferimento all'oggetto selezionato
        TriggerTooltip triggerTooltip = (TriggerTooltip)target;

        // Header per References
        EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
        triggerTooltip.tooltip = (Tooltip)EditorGUILayout.ObjectField(
            new GUIContent("Tooltip", "Riferimento al Tooltip"),
            triggerTooltip.tooltip,
            typeof(Tooltip),
            true
        );

        // Header per Settings
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        triggerTooltip.tooltipMessage = EditorGUILayout.TextField(
            new GUIContent("Tooltip Message", "Messaggio mostrato nel Tooltip"),
            triggerTooltip.tooltipMessage
        );
        triggerTooltip.tooltipDuration = EditorGUILayout.FloatField(
            new GUIContent("Tooltip Duration", "Durata del Tooltip in secondi"),
            triggerTooltip.tooltipDuration
        );

        // Se ci sono cambiamenti, aggiorna l'oggetto
        if (GUI.changed) EditorUtility.SetDirty(triggerTooltip);
    }
}
