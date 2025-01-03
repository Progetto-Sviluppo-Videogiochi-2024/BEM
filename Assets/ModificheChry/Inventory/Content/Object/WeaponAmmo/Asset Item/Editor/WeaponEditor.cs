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
        weapon.nameItem = EditorGUILayout.TextField(new GUIContent("Item Name", "Nome dell'arma"), weapon.nameItem);
        weapon.description = EditorGUILayout.TextField(new GUIContent("Description", "Descrizione dell'arma"), weapon.description);
        weapon.tagType = (Item.ItemTagType)EditorGUILayout.EnumPopup(new GUIContent("Tag Type", "Tipo di tag dell'arma"), weapon.tagType);
        weapon.weight = EditorGUILayout.FloatField(new GUIContent("Weight", "Peso dell'arma"), weapon.weight);
        // weapon.prefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Prefab", "Prefab dell'arma, da assegnare solo per gli oggetti istanziabili nel corpo del pg (es. armi)"), weapon.prefab, typeof(GameObject), false);
        weapon.canDestroy = EditorGUILayout.Toggle(new GUIContent("Can Destroy", "Possibilità di distruggere l'arma (true se distruttibile, false se non distruttibile)"), weapon.canDestroy);
        // Mostra i campi specifici in base al tagType
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Usability Info", EditorStyles.boldLabel);
        weapon.isUsable = EditorGUILayout.Toggle(new GUIContent("Usable", "Usabilità dell'arma (true se usabile, false se non usabile)"), weapon.isUsable);
        weapon.isShooting = EditorGUILayout.Toggle(new GUIContent("Shooting", "Sparabilità dell'arma (true se sparabile, false se non sparabile)"), weapon.isShooting);
        weapon.isPickUp = EditorGUILayout.Toggle(new GUIContent("Pick Up", "Raccoglibilità dell'arma (true se raccoglibile, false se non raccoglibile)"), weapon.isPickUp);
        weapon.isInCraft = EditorGUILayout.Toggle(new GUIContent("In Craft", "Presenza dell'arma nella lista degli oggetti craftabili"), weapon.isInCraft);

        // Mostra i campi specifici in base al tagType
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Inventory Info", EditorStyles.boldLabel);
        weapon.inventorySectionType = (Item.ItemType)EditorGUILayout.EnumPopup(new GUIContent("Inventory Section Type", "Tipo della sezione dell'inventario"), weapon.inventorySectionType);
        // weapon.own = EditorGUILayout.Toggle(new GUIContent("Own", "Possesso dell'arma (true se posseduto, false se non posseduto o non si può possedere)"), weapon.own);
        // weapon.qta = EditorGUILayout.IntField(new GUIContent("Quantity", "Quantità dell'arma"), weapon.qta);
        weapon.icon = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Icon", "Icona dell'arma"), weapon.icon, typeof(Sprite), false);
        weapon.image = (Sprite)EditorGUILayout.ObjectField(new GUIContent("Image", "Immagine dell'arma"), weapon.image, typeof(Sprite), false);

        // Mostra i campi specifici dell'arma
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Weapon Info", EditorStyles.boldLabel);
        
        EditorGUI.BeginDisabledGroup(true);
        weapon.prefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Weapon Prefab", "Prefab dell'arma, istanziato a run-time nel Pickup"), weapon.prefab, typeof(GameObject), true);
        EditorGUI.EndDisabledGroup();

        weapon.weaponType = (Weapon.WeaponType)EditorGUILayout.EnumPopup(new GUIContent("Weapon Type", "Tipo di arma (Melee o Ranged)"), weapon.weaponType);
        weapon.rangeType = (Weapon.RangeType)EditorGUILayout.EnumPopup(new GUIContent("Range Type", "Tipo di raggio (Corto o Medio)"), weapon.rangeType);
        weapon.ammo = (Ammo)EditorGUILayout.ObjectField(new GUIContent("Ammo", "Munizioni dell'arma (se applicabile)"), weapon.ammo, typeof(Ammo), true);
        weapon.distance = EditorGUILayout.FloatField(new GUIContent("Distance", "Distanza massima di tiro dell'arma"), weapon.distance);
        weapon.semiAuto = EditorGUILayout.Toggle(new GUIContent("Semi Auto", "Se l'arma è semiautomatica (cioè spara un colpo per volta)"), weapon.semiAuto);
        weapon.isThrowable = EditorGUILayout.Toggle(new GUIContent("Throwable", "Se l'arma è da lancio (coltelli, bottiglie)"), weapon.isThrowable);

        // Mostra i campi audio dell'arma
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Audio Properties", EditorStyles.boldLabel);
        weapon.fireSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Fire Sound", "Suono di sparo"), weapon.fireSound, typeof(AudioClip), false);
        weapon.magInSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Mag In Sound", "Suono di inserimento del caricatore"), weapon.magInSound, typeof(AudioClip), false);
        weapon.magOutSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Mag Out Sound", "Suono di estrazione del caricatore"), weapon.magOutSound, typeof(AudioClip), false);
        weapon.releaseSlideSound = (AudioClip)EditorGUILayout.ObjectField(new GUIContent("Release Slide Sound", "Suono di rilascio del caricatore"), weapon.releaseSlideSound, typeof(AudioClip), false);

        // Mostra i campi di posizionamento dell'arma nelle mani
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Positioning Weapon in Hands", EditorStyles.boldLabel);
        
        weapon.IdlePosition = EditorGUILayout.Vector3Field(new GUIContent("Idle Position", "Posizione dell'arma in idle"), weapon.IdlePosition);
        Vector3 idleRotationEuler = weapon.IdleRotation.eulerAngles;
        idleRotationEuler = EditorGUILayout.Vector3Field(new GUIContent("Idle Rotation", "Rotazione dell'arma in idle"), idleRotationEuler);
        weapon.IdleRotation = Quaternion.Euler(idleRotationEuler);

        weapon.AimPosition = EditorGUILayout.Vector3Field(new GUIContent("Aim Position", "Posizione dell'arma durante il mirare"), weapon.AimPosition);
        Vector3 aimRotationEuler = weapon.AimRotation.eulerAngles;
        aimRotationEuler = EditorGUILayout.Vector3Field(new GUIContent("Aim Rotation", "Rotazione dell'arma durante il mirare"), aimRotationEuler);
        weapon.AimRotation = Quaternion.Euler(aimRotationEuler);

        weapon.Scale = EditorGUILayout.Vector3Field(new GUIContent("Scale", "Scala dell'arma equipaggiata"), weapon.Scale);


        // Assicura che il cambiamento venga salvato
        if (GUI.changed) EditorUtility.SetDirty(weapon);
    }
}
