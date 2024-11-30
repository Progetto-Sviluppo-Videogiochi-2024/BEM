using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tooltip))]
public class TooltipEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Mostra il commento come tooltip della classe
        EditorGUILayout.HelpBox("Da usare con: trigger e item raccoglibili.", MessageType.Info);

        // Ottieni il riferimento all'oggetto selezionato
        Tooltip tooltip = (Tooltip)target;

        // Header per Settings
        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
        tooltip.offset = EditorGUILayout.Vector3Field(
            new GUIContent("Offset", "Offset del tooltip rispetto alla testa"),
            tooltip.offset
        );

        // Header per References
        EditorGUILayout.LabelField("References", EditorStyles.boldLabel);

        // Mostra head come campo modificabile
        tooltip.head = (Transform)EditorGUILayout.ObjectField(
            new GUIContent("Head", "Riferimento alla testa del personaggio"),
            tooltip.head,
            typeof(Transform),
            true
        );

        // Mostra la mainCamera come readonly
        EditorGUI.BeginDisabledGroup(true); // Disabilita la modifica
        EditorGUILayout.ObjectField(
            new GUIContent("Main Camera", "Riferimento alla camera principale, readonly (settato a runtime)"),
            (Object)tooltip.GetType().GetField("mainCamera", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(tooltip),
            typeof(Camera),
            true
        );
        EditorGUI.EndDisabledGroup();

        // Assicura che eventuali modifiche vengano salvate
        if (GUI.changed)
        {
            EditorUtility.SetDirty(tooltip);
        }
    }
}
