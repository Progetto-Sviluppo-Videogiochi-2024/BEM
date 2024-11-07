using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Ottieni il riferimento all'oggetto Item
        Item item = (Item)target;

        // Mostra i campi generali con i tooltip
        EditorGUILayout.LabelField("General Info", EditorStyles.boldLabel);
        item.nameItem = EditorGUILayout.TextField(new GUIContent("Item Name", "Nome dell'oggetto"), item.nameItem);
        item.tagType = (Item.ItemTagType)EditorGUILayout.EnumPopup(new GUIContent("Tag Type", "Tipo di tag dell'oggetto"), item.tagType);

        if (item.tagType == Item.ItemTagType.Scene) return;

        item.description = EditorGUILayout.TextField(new GUIContent("Description", "Descrizione dell'oggetto"), item.description);

        // Mostra i campi specifici in base al tagType
        if (item.tagType == Item.ItemTagType.Item)
        {
            item.effectType = (Item.ItemEffectType)EditorGUILayout.EnumPopup(new GUIContent("Effect Type", "Tipo di effetto dell'oggetto, solo per gli oggetti consumabili"), item.effectType);
        }

        if (item.isUsable) item.value = EditorGUILayout.IntField(new GUIContent("Value", "Valore usato da effectType"), item.value);

        if (item.inventorySectionType == Item.ItemType.ConsumableEquipable)
        {
            item.qta = EditorGUILayout.IntField(new GUIContent("Quantity", "Quantità dell'oggetto"), item.qta);
            item.weight = EditorGUILayout.FloatField(new GUIContent("Weight", "Peso dell'oggetto"), item.weight);
        }

        if (item.tagType == Item.ItemTagType.Weapon)
            item.prefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Prefab", "Prefab dell'oggetto, da assegnare solo per gli oggetti istanziabili nel corpo del pg (es. armi, oggetti?)"), item.prefab, typeof(GameObject), false);

        if (item.tagType == Item.ItemTagType.Recipe)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.LabelField("Recipe Info", EditorStyles.boldLabel);
            item.ingredientsRecipe = EditorGUILayout.TextField(new GUIContent("Ingredients", "Nome degli ingredienti necessari alla creazione dell'oggetto (separati da ',')"), item.ingredientsRecipe);
            item.qtaIngredientsRecipe = EditorGUILayout.TextField(new GUIContent("Ingredients Quantity", "Quantità degli ingredienti necessari alla creazione dell'oggetto (separati da ',')"), item.qtaIngredientsRecipe);
            item.craftItem = (Item)EditorGUILayout.ObjectField(new GUIContent("Craft Item", "Item.cs dell'oggetto da creare"), item.craftItem, typeof(Item), false);
        }

        // Mostra i campi di usabilità
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Usability Info", EditorStyles.boldLabel);
        item.isUsable = EditorGUILayout.Toggle(new GUIContent("Usable", "Usabilità dell'oggetto (true se usabile, false se non usabile)"), item.isUsable);
        item.isShooting = EditorGUILayout.Toggle(new GUIContent("Shooting", "Sparabilità dell'oggetto (true se sparabile, false se non sparabile)"), item.isShooting);
        item.isPickUp = EditorGUILayout.Toggle(new GUIContent("Pick Up", "Raccoglibilità dell'oggetto (true se raccoglibile, false se non raccoglibile)"), item.isPickUp);
        item.isInCraft = EditorGUILayout.Toggle(new GUIContent("In Craft", "Presenza dell'oggetto nella lista degli oggetti craftabili"), item.isInCraft);
        item.isStackable = EditorGUILayout.Toggle(new GUIContent("Stackable", "Stackabilità = oggetto accumulabile in più quantità nello stesso slot dell'inventario"), item.isStackable);

        // Mostra i campi dell'inventario
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Inventory Info", EditorStyles.boldLabel);
        item.inventorySectionType = (Item.ItemType)EditorGUILayout.EnumPopup(new GUIContent("Inventory Section Type", "Tipo della sezione dell'inventario"), item.inventorySectionType);
        item.own = EditorGUILayout.Toggle(new GUIContent("Own", "Possesso dell'oggetto (true se posseduto, false se non posseduto o non si può possedere)"), item.own);
        item.icon = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Icon", "Icona dell'oggetto"), item.icon, typeof(Sprite), false);
        item.image = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Image", "Immagine dell'oggetto"), item.image, typeof(Sprite), false);

        // Assicura che il cambiamento venga salvato
        if (GUI.changed) EditorUtility.SetDirty(item);
    }
}
