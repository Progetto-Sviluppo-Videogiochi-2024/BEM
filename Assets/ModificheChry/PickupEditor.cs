using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemPickup))]
public class PickupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Pickup Info", EditorStyles.boldLabel);
        ItemPickup itemPickup = (ItemPickup)target;

        // if (itemPickup.item != null) itemPickup.item = (Item)EditorGUILayout.ObjectField(new GUIContent("Item", "Oggetto da raccogliere"), itemPickup.item, typeof(Item), true);
        if (itemPickup.item != null && itemPickup.item.inventorySectionType.Equals(Item.ItemType.ConsumableEquipable))
        {
            itemPickup.triggerTooltip = (TriggerToolTip)EditorGUILayout.ObjectField(new GUIContent("Trigger Tooltip", "Riferimento al trigger del tooltip"), itemPickup.triggerTooltip, typeof(TriggerToolTip), true);
        }

        if (GUI.changed) EditorUtility.SetDirty(itemPickup);
    }
}
