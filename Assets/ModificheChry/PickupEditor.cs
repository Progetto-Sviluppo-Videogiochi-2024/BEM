using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemPickup))]
public class PickupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ItemPickup itemPickup = (ItemPickup)target;
        ItemController itemController = itemPickup.GetComponent<ItemController>();

        if (itemController.item.inventorySectionType.Equals(Item.ItemType.ConsumableEquipable) && !itemController.item.tagType.Equals(Item.ItemTagType.Scene))
        {
            EditorGUILayout.LabelField("Pickup Info", EditorStyles.boldLabel);
            itemPickup.tooltip = (Tooltip)EditorGUILayout.ObjectField(
                new GUIContent("Tooltip", "Riferimento al trigger del tooltip"),
                itemPickup.tooltip,
                typeof(Tooltip),
                true);
        }

        if (GUI.changed) EditorUtility.SetDirty(itemPickup);
    }
}
