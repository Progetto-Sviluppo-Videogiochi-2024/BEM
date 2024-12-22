using UnityEngine;
using static Item;

public class ItemClassManager : MonoBehaviour
{
    [Header("References")]
    #region References
    public Tooltip tooltip; // Riferimento al tooltip
    private Player player; // Riferimento al player
    [HideInInspector] public InventoryItemController itemEquipable; // Oggetto consumabile equipaggiato dall'inventario
    #endregion

    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (itemEquipable == null) return; // Se non equipaggiato, non fare nulla

        if (Input.GetKeyDown(KeyCode.Tab)) UseItem();
    }

    private void UseItem()
    {
        if (itemEquipable.IsAlsoConsumable())
        {
            if (player.health >= player.maxHealth && player.sanitaMentale >= player.maxHealth)
            {
                // DebugLogger.Log($"Tab: {itemEquipable.item.nameItem} - {tooltip}");
                tooltip.ShowTooltip("Non ne ho bisogno!", 5f);
                return;
            }

            if (itemEquipable.item.effectType == ItemEffectType.Health)
            {
                player.UpdateStatusPlayer(itemEquipable.item.value, itemEquipable.item.valueSanita);
                itemEquipable.RemoveItem();
            }
        }
    }
}
