using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Weapon))]
public class WeaponEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Ottieni il riferimento all'oggetto Weapon
        Weapon weapon = (Weapon)target;

        // Mostra i campi generali con i tooltip
        EditorGUILayout.LabelField("General Info", EditorStyles.boldLabel);
        weapon.nameItem = EditorGUILayout.TextField(new GUIContent("Item Name", "Nome dell'oggetto"), weapon.nameItem);
        weapon.description = EditorGUILayout.TextField(new GUIContent("Description", "Descrizione dell'oggetto"), weapon.description);
        weapon.tagType = (Item.ItemTagType)EditorGUILayout.EnumPopup(new GUIContent("Tag Type", "Tipo di tag dell'oggetto"), weapon.tagType);
        weapon.weight = EditorGUILayout.FloatField(new GUIContent("Weight", "Peso dell'oggetto"), weapon.weight);
        weapon.prefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Prefab", "Prefab dell'oggetto, da assegnare solo per gli oggetti istanziabili nel corpo del pg (es. armi)"), weapon.prefab, typeof(GameObject), false);

        // Mostra i campi specifici in base al tagType
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Usability Info", EditorStyles.boldLabel);
        weapon.isUsable = EditorGUILayout.Toggle(new GUIContent("Usable", "Usabilità dell'oggetto (true se usabile, false se non usabile)"), weapon.isUsable);
        weapon.isShooting = EditorGUILayout.Toggle(new GUIContent("Shooting", "Sparabilità dell'oggetto (true se sparabile, false se non sparabile)"), weapon.isShooting);
        weapon.isPickUp = EditorGUILayout.Toggle(new GUIContent("Pick Up", "Raccoglibilità dell'oggetto (true se raccoglibile, false se non raccoglibile)"), weapon.isPickUp);
        weapon.isInCraft = EditorGUILayout.Toggle(new GUIContent("In Craft", "Presenza dell'oggetto nella lista degli oggetti craftabili"), weapon.isInCraft);

        // Mostra i campi specifici in base al tagType
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Inventory Info", EditorStyles.boldLabel);
        weapon.inventorySectionType = (Item.ItemType)EditorGUILayout.EnumPopup(new GUIContent("Inventory Section Type", "Tipo della sezione dell'inventario"), weapon.inventorySectionType);
        weapon.own = EditorGUILayout.Toggle(new GUIContent("Own", "Possesso dell'oggetto (true se posseduto, false se non posseduto o non si può possedere)"), weapon.own);
        weapon.qta = EditorGUILayout.IntField(new GUIContent("Quantity", "Quantità dell'oggetto"), weapon.qta);
        weapon.icon = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Icon", "Icona dell'oggetto"), weapon.icon, typeof(Sprite), false);
        weapon.image = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Image", "Immagine dell'oggetto"), weapon.image, typeof(Sprite), false);

        // Mostra i campi specifici dell'arma
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Weapon Info", EditorStyles.boldLabel);
        weapon.weaponType = (Weapon.WeaponType)EditorGUILayout.EnumPopup(new GUIContent("Weapon Type", "Tipo di arma (Melee o Ranged)"), weapon.weaponType);
        weapon.rangeType = (Weapon.RangeType)EditorGUILayout.EnumPopup(new GUIContent("Range Type", "Tipo di raggio (Corto o Medio)"), weapon.rangeType);
        weapon.ammo = (Ammo)EditorGUILayout.ObjectField(new GUIContent("Ammo", "Munizioni dell'arma (se applicabile)"), weapon.ammo, typeof(Ammo), true);
        weapon.isThrowable = EditorGUILayout.Toggle(new GUIContent("Throwable", "Se l'arma è da lancio (coltelli, bottiglie)"), weapon.isThrowable);

        // Mostra i campi di posizionamento dell'arma nelle mani
        EditorGUILayout.LabelField("Positioning Weapon in Hands", EditorStyles.boldLabel);
        Vector3 equippedRotationEuler = weapon.equippedRotation.eulerAngles;
        equippedRotationEuler = EditorGUILayout.Vector3Field(new GUIContent("Equipped Rotation", "Rotazione dell'arma equipaggiata"), equippedRotationEuler);
        weapon.equippedRotation = Quaternion.Euler(equippedRotationEuler);
        Vector3 refRightHandGripRotationEuler = weapon.refRightHandGripRotation.eulerAngles;
        refRightHandGripRotationEuler = EditorGUILayout.Vector3Field(new GUIContent("Right Hand Grip Rotation", "Rotazione della mano destra per l'impugnatura"), refRightHandGripRotationEuler);
        weapon.refRightHandGripRotation = Quaternion.Euler(refRightHandGripRotationEuler);
        Vector3 refLeftHandGripRotationEuler = weapon.refLeftHandGripRotation.eulerAngles;
        refLeftHandGripRotationEuler = EditorGUILayout.Vector3Field(new GUIContent("Left Hand Grip Rotation", "Rotazione della mano sinistra per l'impugnatura"), refLeftHandGripRotationEuler);
        weapon.refLeftHandGripRotation = Quaternion.Euler(refLeftHandGripRotationEuler);

        // Assicura che il cambiamento venga salvato
        if (GUI.changed) EditorUtility.SetDirty(weapon);
    }
}
