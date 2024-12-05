using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ammo))]
public class AmmoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Ottieni il riferimento all'oggetto Weapon
        Ammo ammo = (Ammo)target;

        // Mostra i campi generali con i tooltip
        EditorGUILayout.LabelField("General Info", EditorStyles.boldLabel);
        ammo.nameItem = EditorGUILayout.TextField(new GUIContent("Item Name", "Nome del proiettile"), ammo.nameItem);
        ammo.description = EditorGUILayout.TextField(new GUIContent("Description", "Descrizione del proiettile"), ammo.description);
        ammo.tagType = (Item.ItemTagType)EditorGUILayout.EnumPopup(new GUIContent("Tag Type", "Tipo di tag del proiettile"), ammo.tagType);
        ammo.weight = EditorGUILayout.FloatField(new GUIContent("Weight", "Peso del proiettile"), ammo.weight);
        // weapon.prefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Prefab", "Prefab dell'oggetto, da assegnare solo per gli oggetti istanziabili nel corpo del pg (es. armi)"), weapon.prefab, typeof(GameObject), false);
        ammo.canDestroy = EditorGUILayout.Toggle(new GUIContent("Can Destroy", "Possibilità di distruggere l'oggetto"), ammo.canDestroy);

        // Mostra i campi specifici in base al tagType
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Usability Info", EditorStyles.boldLabel);
        // ammo.isUsable = EditorGUILayout.Toggle(new GUIContent("Usable", "Usabilità dell'oggetto (true se usabile, false se non usabile)"), ammo.isUsable);
        // ammo.isShooting = EditorGUILayout.Toggle(new GUIContent("Shooting", "Sparabilità dell'oggetto (true se sparabile, false se non sparabile)"), ammo.isShooting);
        ammo.isPickUp = EditorGUILayout.Toggle(new GUIContent("Pick Up", "Raccoglibilità del proiettile (true se raccoglibile, false se non raccoglibile)"), ammo.isPickUp);
        // ammo.isInCraft = EditorGUILayout.Toggle(new GUIContent("In Craft", "Presenza dell'oggetto nella lista degli oggetti craftabili"), ammo.isInCraft);

        // Mostra i campi specifici in base al tagType
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Inventory Info", EditorStyles.boldLabel);
        ammo.inventorySectionType = (Item.ItemType)EditorGUILayout.EnumPopup(new GUIContent("Inventory Section Type", "Tipo della sezione dell'inventario"), ammo.inventorySectionType);
        // weapon.own = EditorGUILayout.Toggle(new GUIContent("Own", "Possesso dell'oggetto (true se posseduto, false se non posseduto o non si può possedere)"), weapon.own);
        // weapon.qta = EditorGUILayout.IntField(new GUIContent("Quantity", "Quantità dell'oggetto"), weapon.qta);
        ammo.icon = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Icon", "Icona del proiettile"), ammo.icon, typeof(Sprite), false);
        ammo.image = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Image", "Immagine del proiettile"), ammo.image, typeof(Sprite), false);

        // Mostra i campi specifici dell'arma
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Ammo Info", EditorStyles.boldLabel);
        ammo.ammoPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Ammo Prefab", "Prefab del proiettile"), ammo.ammoPrefab, typeof(GameObject), true);
        ammo.ammoType = (Ammo.AmmoType)EditorGUILayout.EnumPopup(new GUIContent("Ammo Type", "Tipo di proiettile"), ammo.ammoType);
        ammo.nAmmo = EditorGUILayout.IntField(new GUIContent("Clipsize Ammo", "Numero di proiettili per caricatore"), ammo.nAmmo);
        ammo.maxAmmo = EditorGUILayout.IntField(new GUIContent("Totals Ammo", "Numero massimo di proiettili"), ammo.maxAmmo);
        ammo.damageAmmo = EditorGUILayout.FloatField(new GUIContent("Damage Ammo", "Danno del proiettile"), ammo.damageAmmo);

        // Assicura che il cambiamento venga salvato
        if (GUI.changed) EditorUtility.SetDirty(ammo);
    }
}
